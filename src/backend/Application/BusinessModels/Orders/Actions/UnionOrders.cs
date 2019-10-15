using System;
using System.Collections.Generic;
using System.Linq;
using Application.Shared;
using DAL;
using DAL.Services;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Объеденить заказы
    /// </summary>
    public class UnionOrders : IGroupAppAction<Order>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService dataService;

        public UnionOrders(ICommonDataService dataService, IHistoryService historyService)
        {
            this.dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Orange;
        }
        
        public AppColor Color { get; set; }
        public AppActionResult Run(CurrentUserDto user, IEnumerable<Order> orders)
        {
            var shippingsCount = this.dataService.GetDbSet<Shipping>().Count();
            var tempRange = FindCommonTempRange(orders);
            decimal? downtime = orders.Any(o => o.TrucksDowntime.HasValue) ? orders.Sum(o => o.TrucksDowntime ?? 0) : (decimal?)null;
            int? palletsCount = orders.Any(o => o.PalletsCount.HasValue) ? orders.Sum(o => o.PalletsCount ?? 0) : (int?)null;
            int? actualPalletsCount = orders.Any(o => o.ActualPalletsCount.HasValue) ? orders.Sum(o => o.ActualPalletsCount ?? 0) : (int?)null;
            int? confirmedPalletsCount = orders.Any(o => o.ConfirmedPalletsCount.HasValue) ? orders.Sum(o => o.ConfirmedPalletsCount ?? 0) : (int?)null;
            decimal? weight = orders.Any(o => o.WeightKg.HasValue) ? orders.Sum(o => o.WeightKg ?? 0) : (decimal?)null;
            decimal? actualWeight = orders.Any(o => o.ActualWeightKg.HasValue) ? orders.Sum(o => o.ActualWeightKg ?? 0) : (decimal?)null;

            var shipping = new Shipping
            {
                Status = ShippingState.ShippingCreated,
                Id = Guid.NewGuid(),
                ShippingNumber = string.Format("SH{0:000000}", shippingsCount + 1),
                ShippingCreationDate = DateTime.Now
            };

            var setter = new FieldSetter<Shipping>(shipping, _historyService);

            setter.UpdateField(s => s.DeliveryType, DeliveryType.Delivery);
            setter.UpdateField(s => s.TarifficationType, TarifficationType.Ftl);
            setter.UpdateField(s => s.TemperatureMin, tempRange?.Key);
            setter.UpdateField(s => s.TemperatureMax, tempRange?.Value);
            setter.UpdateField(s => s.PalletsCount, palletsCount);
            setter.UpdateField(s => s.ActualPalletsCount, actualPalletsCount);
            setter.UpdateField(s => s.ConfirmedPalletsCount, confirmedPalletsCount);
            setter.UpdateField(s => s.WeightKg, weight);
            setter.UpdateField(s => s.ActualWeightKg, actualWeight);
            setter.UpdateField(s => s.TrucksDowntime, downtime);

            _historyService.Save(shipping.Id, "shippingSetCreated", shipping.ShippingNumber);
            setter.SaveHistoryLog();

            this.dataService.GetDbSet<Shipping>().Add(shipping);

            foreach (var order in orders)
            {
                order.ShippingId = shipping.Id;
                order.ShippingNumber = shipping.ShippingNumber;
                order.Status = OrderState.InShipping;

                var ordSetter = new FieldSetter<Order>(order, _historyService);

                ordSetter.UpdateField(o => o.ShippingStatus, VehicleState.VehicleWaiting);
                ordSetter.UpdateField(o => o.DeliveryStatus, VehicleState.VehicleEmpty);

                _historyService.Save(order.Id, "orderSetInShipping", order.OrderNumber, shipping.ShippingNumber);
                ordSetter.SaveHistoryLog();
            }

            this.dataService.SaveChanges();

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetCreated".translate(user.Language, shipping.ShippingNumber)
            };
        }

        private KeyValuePair<int, int>? FindCommonTempRange(IEnumerable<Order> orders)
        {
            if (!orders.Any() || orders.Any(o => o.TemperatureMin == null || o.TemperatureMax == null))
            {
                return null;
            }

            Order firstOrder = orders.First();
            KeyValuePair<int, int> result = new KeyValuePair<int, int>(firstOrder.TemperatureMin.Value, firstOrder.TemperatureMax.Value);

            foreach (Order order in orders.Skip(1))
            {
                if (order.TemperatureMin > result.Value || order.TemperatureMax < result.Key)
                {
                    return null;
                }
                result = new KeyValuePair<int, int>(Math.Max(result.Key, order.TemperatureMin.Value), Math.Min(result.Value, order.TemperatureMax.Value));
            }

            return result;
        }

        public bool IsAvailable(Role role, IEnumerable<Order> target)
        {
            return target.All(entity => entity.Status == OrderState.Created && (role.Name == "Administrator" || role.Name == "TransportCoordinator"));
        }
    }
}
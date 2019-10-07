using System;
using System.Collections.Generic;
using System.Linq;
using Application.Shared;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Объеденить заказы
    /// </summary>
    public class UnionOrders : IGroupAppAction<Order>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public UnionOrders(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Orange;
        }
        
        public AppColor Color { get; set; }
        public AppActionResult Run(User user, IEnumerable<Order> orders)
        {
            var shippingsCount = db.Shippings.Count();
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
                DeliveryType = DeliveryType.Delivery,
                TarifficationType = TarifficationType.Ftl,
                TemperatureMin = tempRange?.Key,
                TemperatureMax = tempRange?.Value,
                PalletsCount = palletsCount,
                ActualPalletsCount = actualPalletsCount,
                ConfirmedPalletsCount = confirmedPalletsCount,
                WeightKg = weight,
                ActualWeightKg = actualWeight,
                TrucksDowntime = downtime
            };
            db.Shippings.Add(shipping);

            foreach (var order in orders)
            {
                var setter = new FieldSetter<Order>(order, _historyService);

                setter.UpdateField(o => o.Status, OrderState.InShipping, ignoreChanges: true);
                setter.UpdateField(o => o.ShippingId, shipping.Id, ignoreChanges: true);
                setter.UpdateField(o => o.ShippingStatus, VehicleState.VehicleWaiting);
                setter.UpdateField(o => o.DeliveryStatus, VehicleState.VehicleEmpty);

                _historyService.Save(order.Id, "orderSetInShipping", order.OrderNumber, shipping.ShippingNumber);
                setter.SaveHistoryLog();
            }
            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = $"Созданна перевозка {shipping.ShippingNumber}"
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
using Application.BusinessModels.Shared.Actions;
using Application.Shared;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Объеденить заказы
    /// </summary>
    public class UnionOrders : UnionOrdersBase, IGroupAppAction<Order>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService _dataService;

        public UnionOrders(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Orange;
        }
        
        public AppColor Color { get; set; }
        public AppActionResult Run(CurrentUserDto user, IEnumerable<Order> orders)
        {
            var shippingDbSet = _dataService.GetDbSet<Shipping>();
            var shippingsCount = shippingDbSet.Count();

            var shipping = new Shipping
            {
                Status = ShippingState.ShippingCreated,
                Id = Guid.NewGuid(),
                ShippingNumber = string.Format("SH{0:000000}", shippingsCount + 1),
                ShippingCreationDate = DateTime.Now
            };

            _historyService.Save(shipping.Id, "shippingSetCreated", shipping.ShippingNumber);
            
            var setter = new FieldSetter<Shipping>(shipping, _historyService);

            setter.UpdateField(s => s.DeliveryType, DeliveryType.Delivery);
            setter.UpdateField(s => s.TarifficationType, TarifficationType.Ftl);
<<<<<<< HEAD
            
=======
            setter.UpdateField(s => s.TemperatureMin, tempRange?.Key);
            setter.UpdateField(s => s.TemperatureMax, tempRange?.Value);
            setter.UpdateField(s => s.PalletsCount, palletsCount);
            setter.UpdateField(s => s.ActualPalletsCount, actualPalletsCount);
            setter.UpdateField(s => s.ConfirmedPalletsCount, confirmedPalletsCount);
            setter.UpdateField(s => s.WeightKg, weight);
            setter.UpdateField(s => s.ActualWeightKg, actualWeight);
            setter.UpdateField(s => s.TrucksDowntime, downtime);

            var ordersLoadingArrivalTime = orders.Select(i => i.LoadingArrivalTime).FirstOrDefault();
            var updateLoadingArrivalTime = ordersLoadingArrivalTime.HasValue && orders.All(i => i.LoadingArrivalTime == ordersLoadingArrivalTime);

            var ordersLoadingDepartureTime = orders.Select(i => i.LoadingDepartureTime).FirstOrDefault();
            var updateLoadingDepartureTime = ordersLoadingDepartureTime.HasValue && orders.All(i => i.LoadingDepartureTime == ordersLoadingDepartureTime);

            if (updateLoadingArrivalTime)
            {
                setter.UpdateField(s => s.LoadingArrivalTime, ordersLoadingArrivalTime);
            }

            if (updateLoadingDepartureTime)
            {
                setter.UpdateField(s => s.LoadingDepartureTime, ordersLoadingDepartureTime);
            }

            _historyService.Save(shipping.Id, "shippingSetCreated", shipping.ShippingNumber);
>>>>>>> origin/develop
            setter.SaveHistoryLog();

            shippingDbSet.Add(shipping);
            
            UnionOrderInShipping(orders, shipping, shippingDbSet, _historyService);
            
            _dataService.SaveChanges();

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetCreated".Translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(IEnumerable<Order> target)
        {
            return target.All(entity => entity.Status == OrderState.Created);
        }
    }
}
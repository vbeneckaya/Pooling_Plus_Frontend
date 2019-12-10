using Application.BusinessModels.Shared.Actions;
using Application.Services.Shippings;
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
        private readonly IChangeTrackerFactory _changeTrackerFactory;

        public UnionOrders(ICommonDataService dataService, 
                           IHistoryService historyService, 
                           IShippingCalculationService shippingCalculationService,
                           IChangeTrackerFactory changeTrackerFactory)
            : base(dataService, shippingCalculationService)
        {
            _historyService = historyService;
            _changeTrackerFactory = changeTrackerFactory;
            Color = AppColor.Orange;
        }
        
        public AppColor Color { get; set; }
        public AppActionResult Run(CurrentUserDto user, IEnumerable<Order> orders)
        {
            var shippingDbSet = _dataService.GetDbSet<Shipping>();

            var shipping = new Shipping
            {
                Status = ShippingState.ShippingCreated,
                Id = Guid.NewGuid(),
                ShippingNumber = ShippingNumberProvider.GetNextShippingNumber(),
                ShippingCreationDate = DateTime.UtcNow
            };

            _historyService.Save(shipping.Id, "shippingSetCreated", shipping.ShippingNumber);
            
            shipping.DeliveryType = DeliveryType.Delivery;
            shipping.TarifficationType = TarifficationType.Ftl;
            
            shippingDbSet.Add(shipping);
            
            UnionOrderInShipping(orders, orders, shipping, _historyService);

            var changes = _dataService.GetChanges<Shipping>().FirstOrDefault(x => x.Entity.Id == shipping.Id);
            var changeTracker = _changeTrackerFactory.CreateChangeTracker();
            changeTracker.LogTrackedChanges(changes);

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetCreated".Translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(IEnumerable<Order> target)
        {
            return target.All(order => order.Status == OrderState.Confirmed && 
                                       (!order.DeliveryType.HasValue || order.DeliveryType.Value == DeliveryType.Delivery));
        }
    }
}
using Application.BusinessModels.Shared.Actions;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Services.Shippings;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Объеденить заказы
    /// </summary>
    [ActionGroup(nameof(Order)), OrderNumber(4)]
    public class UnionOrders : UnionOrdersBase, IGroupAppAction<Order>
    {
        private readonly IHistoryService _historyService;
        private readonly IChangeTrackerFactory _changeTrackerFactory;
        private readonly IShippingGetRouteService _shippingGetRouteService;

        public UnionOrders(ICommonDataService dataService, 
                           IHistoryService historyService,
                           IShippingCalculationService shippingCalculationService,
                           IChangeTrackerFactory changeTrackerFactory,
                           IShippingGetRouteService shippingGetRouteService
                           )
            : base(dataService, shippingCalculationService, shippingGetRouteService)
        {
            _historyService = historyService;
            _changeTrackerFactory = changeTrackerFactory;
            _shippingGetRouteService = shippingGetRouteService;
            Color = AppColor.Orange;
            Description = "Объеденить накладные в одну перевозку";
        }
        
        public AppColor Color { get; set; }
        public string Description { get; set; }

        public AppActionResult Run(CurrentUserDto user, IEnumerable<Order> orders)
        {
            var shippingDbSet = _dataService.GetDbSet<Shipping>();

            var poolingInfo = "Эту перевозку можно отправить в Pooling";
            
            var shipping = new Shipping
            {
                Status = ShippingState.ShippingCreated,
                PoolingState = ShippingPoolingState.PoolingAvailable,
                PoolingInfo = poolingInfo,
                Id = Guid.NewGuid(),
                ShippingNumber = ShippingNumberProvider.GetNextShippingNumber(),
                ProviderId = user.ProviderId,
                ShippingCreationDate = DateTime.UtcNow,
                UserCreatorId = user.Id.Value
                
            };

            _historyService.Save(shipping.Id, "shippingSetCreated", shipping.ShippingNumber);
            
            shipping.DeliveryType = DeliveryType.Delivery;

            shippingDbSet.Add(shipping);
            
            UnionOrderInShipping(orders, orders, shipping, _historyService);
            
            var changes = _dataService.GetChanges<Shipping>().FirstOrDefault(x => x.Entity.Id == shipping.Id);
            var changeTracker = _changeTrackerFactory.CreateChangeTracker()
                                                     .TrackAll<Shipping>()
                                                     .Remove<Shipping>(x => x.Id);
            changeTracker.LogTrackedChanges(changes);

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetCreated".Translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(IEnumerable<Order> target)
        {
            return target.All(order => order.Status == OrderState.Created && 
                                       (!order.DeliveryType.HasValue || order.DeliveryType.Value == DeliveryType.Delivery));
        }
    }
}
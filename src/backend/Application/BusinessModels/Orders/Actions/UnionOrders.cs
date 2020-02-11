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
using Integrations.Pooling;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Объеденить заказы
    /// </summary>
    [ActionGroup(nameof(Order)), OrderNumber(4)]
    public class UnionOrders : UnionOrdersBase, IGroupAppAction<Order>
    {
        private readonly IHistoryService _historyService;
        private readonly IShippingTarifficationTypeDeterminer _shippingTarifficationTypeDeterminer;
        private readonly IChangeTrackerFactory _changeTrackerFactory;

        public UnionOrders(ICommonDataService dataService, 
                           IHistoryService historyService, 
                           IShippingTarifficationTypeDeterminer shippingTarifficationTypeDeterminer, 
                           IShippingCalculationService shippingCalculationService,
                           IChangeTrackerFactory changeTrackerFactory
                           )
            : base(dataService, shippingCalculationService)
        {
            _historyService = historyService;
            _shippingTarifficationTypeDeterminer = shippingTarifficationTypeDeterminer;
            _changeTrackerFactory = changeTrackerFactory;
            Color = AppColor.Orange;
            Description = "Объеденить накладные в одну перевозку";
        }
        
        public AppColor Color { get; set; }
        public string Description { get; set; }

        public AppActionResult Run(CurrentUserDto user, IEnumerable<Order> orders)
        {
            var shippingDbSet = _dataService.GetDbSet<Shipping>();

            var shipping = new Shipping
            {
                Status = ShippingState.ShippingCreated,
                Id = Guid.NewGuid(),
                ShippingNumber = ShippingNumberProvider.GetNextShippingNumber(),
                ShippingCreationDate = DateTime.UtcNow,
                UserCreatorId = user.Id.Value
            };

            _historyService.Save(shipping.Id, "shippingSetCreated", shipping.ShippingNumber);
            
            shipping.DeliveryType = DeliveryType.Delivery;
            shipping.TarifficationType = _shippingTarifficationTypeDeterminer.GetTarifficationTypeForOrders(orders);

            shippingDbSet.Add(shipping);

            var currentUser = _dataService.GetById<User>(user.Id.Value);

            if (currentUser.IsPoolingIntegrated() && shipping.CarrierId != null)
            {
                using (var poolingIntegration = new PoolingIntegration(currentUser, _dataService))
                {
                    var poolingInfo = poolingIntegration.GetInfoFor(shipping);
                    
                    if (poolingInfo.IsAvailable)
                        shipping.PoolingState = ShippingPoolingState.PoolingAvailable;
                    else
                        shipping.PoolingState = null;

                    shipping.PoolingInfo = poolingInfo.MessageField;
                }
            }
            
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
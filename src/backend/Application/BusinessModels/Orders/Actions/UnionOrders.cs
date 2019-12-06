using Application.BusinessModels.Shared.Actions;
using Application.Services.Shippings;
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

        public UnionOrders(ICommonDataService dataService, IHistoryService historyService)
            : base(dataService)
        {
            _historyService = historyService;
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
            
            var setter = new FieldSetter<Shipping>(shipping, _historyService);

            setter.UpdateField(s => s.DeliveryType, DeliveryType.Delivery);
            setter.UpdateField(s => s.TarifficationType, TarifficationType.Ftl);
            
            setter.SaveHistoryLog();

            shippingDbSet.Add(shipping);
            
            UnionOrderInShipping(orders, shipping, shippingDbSet, _historyService);

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
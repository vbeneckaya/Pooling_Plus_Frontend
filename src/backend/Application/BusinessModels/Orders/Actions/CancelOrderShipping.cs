using Application.BusinessModels.Shared.Actions;
using Application.BusinessModels.Shippings.Actions;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Отменить перевозку
    /// </summary>
    [ActionGroup(nameof(Shipping)), OrderNumber(16), ActionAccess(ActionAccess.GridOnly)]
    public class CancelOrderShipping : IAppAction<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly CancelShipping _shippingAction;

        public AppColor Color { get; set; }
        public string Description { get; set; }

        public CancelOrderShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _shippingAction = new CancelShipping(dataService, historyService);
            Color = _shippingAction.Color;
            Description = "Отменить перевозку и убрать её из накладных";
        }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            if (order?.ShippingId == null)
            {
                return new AppActionResult
                {
                    IsError = true,
                    Message = "orderShippingNotFound".Translate(user.Language)
                };
            }

            var shipping = _dataService.GetById<Shipping>(order.ShippingId.Value);

            return _shippingAction.Run(user, shipping);
        }

        public bool IsAvailable(Order order)
        {
            return (order.OrderShippingStatus == ShippingState.ShippingCreated
                    || order.OrderShippingStatus == ShippingState.ShippingRequestSent
                    || order.OrderShippingStatus == ShippingState.ShippingConfirmed) ;
            
        }
    }
}
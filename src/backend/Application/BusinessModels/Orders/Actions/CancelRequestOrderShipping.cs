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
    /// Отменить заявку
    /// </summary>
    [ActionGroup(nameof(Shipping)), OrderNumber(15), ActionAccess(ActionAccess.GridOnly)]
    public class CancelRequestOrderShipping : IAppAction<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly CancelRequestShipping _shippingAction;

        public AppColor Color { get; set; }
        public string Description { get; set; }

        public CancelRequestOrderShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _shippingAction = new CancelRequestShipping(dataService, historyService);
            Color = _shippingAction.Color;
            Description = "Отклонить заявку и переместить в статус \"Создана\"";
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
            return _shippingAction.IsAvailable(order.OrderShippingStatus);
        }
    }
}
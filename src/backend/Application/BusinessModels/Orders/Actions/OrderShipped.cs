using Application.BusinessModels.Shared.Actions;
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
    /// Заказ отгружен
    /// </summary>
    [ActionGroup(nameof(Order)), OrderNumber(5)]
    public class OrderShipped : IAppAction<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public OrderShipped(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Orange;
            Description = "Пометить наклданую как отгруженную";
        }

        public AppColor Color { get; set; }
        public string Description { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            order.Status = OrderState.Shipped;

            _historyService.Save(order.Id, "orderSetShipped", order.OrderNumber);
            
            return new AppActionResult
            {
                IsError = false,
                Message = "orderSetShipped".Translate(user.Language, order.OrderNumber)
            };
        }

        public bool IsAvailable(Order order)
        {
            return (order.Status == OrderState.InShipping && (!order.DeliveryType.HasValue || order.DeliveryType.Value == DeliveryType.Delivery)) ||
                   (order.Status == OrderState.Created && (order.DeliveryType.HasValue && order.DeliveryType.Value == DeliveryType.SelfDelivery));
        }
    }
}
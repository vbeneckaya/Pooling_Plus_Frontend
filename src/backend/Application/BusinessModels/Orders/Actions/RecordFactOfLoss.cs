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
    /// Заказ потерян
    /// </summary>
    [ActionGroup(nameof(Order)), OrderNumber(10)]
    public class RecordFactOfLoss : IAppAction<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public RecordFactOfLoss(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Red;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            order.Status = OrderState.Lost;

            _historyService.Save(order.Id, "orderSetLost", order.OrderNumber);
            
            return new AppActionResult
            {
                IsError = false,
                Message = "orderSetLost".Translate(user.Language, order.OrderNumber)
            };
        }

        public bool IsAvailable(Order order)
        {
            return order.Status == OrderState.Shipped &&
                   (!order.DeliveryType.HasValue || order.DeliveryType.Value == DeliveryType.Delivery);
        }
    }
}
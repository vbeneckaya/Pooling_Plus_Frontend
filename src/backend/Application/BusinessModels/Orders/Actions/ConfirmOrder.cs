using Application.BusinessModels.Shared.Actions;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Отменить заказ
    /// </summary>
    public class ConfirmOrder : IAppAction<Order>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService _dataService;

        public ConfirmOrder(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Red;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            order.Status = OrderState.Confirmed;

            _historyService.Save(order.Id, "orderSetConfirmed", order.OrderNumber);

            _dataService.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = "orderSetConfirmed".Translate(user.Language, order.OrderNumber)
            };
        }

        public bool IsAvailable(Order order)
        {
            return order.Status == OrderState.Created;
        }
    }
}
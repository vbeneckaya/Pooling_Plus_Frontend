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
    /// Полный возврат
    /// </summary>
    [ActionGroup(nameof(Order)), OrderNumber(9)]
    public class FullReject : IAppAction<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public FullReject(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Orange;
            Description = "Установить \"Полный возврат\" для накладной";
        }

        public AppColor Color { get; set; }
        public string Description { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            order.Status = OrderState.FullReturn;

            _historyService.Save(order.Id, "orderSetFullReturn", order.OrderNumber);
            
            return new AppActionResult
            {
                IsError = false,
                Message = "orderSetFullReturn".Translate(user.Language, order.OrderNumber)
            };
        }

        public bool IsAvailable(Order order)
        {
            return order.Status == OrderState.Shipped;
        }
    }
}
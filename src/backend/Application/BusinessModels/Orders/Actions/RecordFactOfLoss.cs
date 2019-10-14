using DAL;
using Domain;
using Domain.Enums;
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
    public class RecordFactOfLoss : IAppAction<Order>
    {
        private readonly ICommonDataService dataService;
        private readonly IHistoryService _historyService;

        public RecordFactOfLoss(ICommonDataService dataService, IHistoryService historyService)
        {
            this.dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Red;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            order.Status = OrderState.Lost;

            _historyService.Save(order.Id, "orderSetLost", order.OrderNumber);

            dataService.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = "orderSetLost".translate(user.Language, order.OrderNumber)
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Shipped && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
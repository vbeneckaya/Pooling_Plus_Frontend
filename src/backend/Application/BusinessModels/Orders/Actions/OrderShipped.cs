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
    /// Заказ доставлен
    /// </summary>
    public class OrderShipped : IAppAction<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public OrderShipped(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Orange;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            order.Status = OrderState.Shipped;

            _historyService.Save(order.Id, "orderSetShipped", order.OrderNumber);

            _dataService.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = "orderSetShipped".translate(user.Language, order.OrderNumber)
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return (order.Status == OrderState.InShipping) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
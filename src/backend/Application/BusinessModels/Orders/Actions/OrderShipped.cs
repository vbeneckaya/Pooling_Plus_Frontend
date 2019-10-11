using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Заказ доставлен
    /// </summary>
    public class OrderShipped : IAppAction<Order>
    {
        private readonly ICommonDataService dataService;
        private readonly IHistoryService _historyService;

        public OrderShipped(ICommonDataService dataService, IHistoryService historyService)
        {
            this.dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Orange;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = OrderState.Shipped;

            _historyService.Save(order.Id, "orderSetShipped", order.OrderNumber);

            dataService.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Заказ {order.OrderNumber} отгружен"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return (order.Status == OrderState.InShipping) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
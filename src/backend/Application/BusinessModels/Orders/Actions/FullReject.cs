using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Полный возврат
    /// </summary>
    public class FullReject : IAppAction<Order>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public FullReject(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Orange;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = OrderState.FullReturn;

            _historyService.Save(order.Id, "orderSetFullReturn", order.OrderNumber);

            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Полный повзрат по заказу {order.OrderNumber}"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Shipped && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
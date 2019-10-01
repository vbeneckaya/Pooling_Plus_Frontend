using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    /// <summary>
    /// Полный возврат
    /// </summary>
    public class FullReject : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public FullReject(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Orange;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = OrderState.FullReturn;
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
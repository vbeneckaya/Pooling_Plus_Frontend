using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    /// <summary>
    /// Заказ отгружен
    /// </summary>
    public class OrderDelivered : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public OrderDelivered(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Green;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = OrderState.Delivered;
            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Заказ {order.OrderNumber} доставлен"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Shipped && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
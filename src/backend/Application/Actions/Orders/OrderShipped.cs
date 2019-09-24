using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    public class OrderShipped : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public OrderShipped(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Orange;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = OrderState.Shipped;
            
            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Заказ {order.SalesOrderNumber} отгружен"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return (order.Status == OrderState.InShipping) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
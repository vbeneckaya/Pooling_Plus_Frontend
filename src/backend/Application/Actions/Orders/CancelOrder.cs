using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    /// <summary>
    /// Отменить заказ
    /// </summary>
    public class CancelOrder : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public CancelOrder(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Red;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = OrderState.Canceled;
            
            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Заказ {order.OrderNumber} отменён"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return (order.Status == OrderState.Created || order.Status == OrderState.Draft) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
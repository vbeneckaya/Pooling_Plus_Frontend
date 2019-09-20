using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    public class Cancel : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public Cancel(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Red;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = "Отменён";
            
            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Заказ {order.Id} отменён "
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return (order.Status == "Создан" || order.Status == "Не проверен") && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
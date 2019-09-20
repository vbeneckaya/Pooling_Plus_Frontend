using System;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    public class RecordFactOfLoss : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public RecordFactOfLoss(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = "Потерян";
            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Заказ {order.Id} потерян"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == "В перевозке" && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
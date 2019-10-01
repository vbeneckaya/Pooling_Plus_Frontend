using System;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    /// <summary>
    /// Заказ потерян
    /// </summary>
    public class RecordFactOfLoss : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public RecordFactOfLoss(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Red;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = OrderState.Lost;
            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Заказ {order.OrderNumber} потерян"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Shipped && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
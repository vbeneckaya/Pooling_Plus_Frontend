using System;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    public class SendToArchive : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public SendToArchive(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = OrderState.Archive;
            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Созданна перевозка {order.SalesOrderNumber}"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Delivered && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
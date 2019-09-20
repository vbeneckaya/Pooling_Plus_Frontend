using System;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    public class Archive : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public 
            Archive(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = "В архиве";
            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Созданна перевозка {order.Id}"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == "Доставлен" && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
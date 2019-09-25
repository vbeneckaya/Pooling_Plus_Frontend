using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Microsoft.EntityFrameworkCore.Internal;

namespace Application.Actions.Orders
{
    /// <summary>
    /// Создать перевозку для каждого заказа
    /// </summary>
    public class CreateShippingForeach : IGroupAppAction<Order>
    {
        private readonly AppDbContext db;

        public CreateShippingForeach(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, IEnumerable<Order> orders)
        {
            var createShipping = new CreateShipping(db);
            foreach (var order in orders) 
                createShipping.Run(user, order);

            return new AppActionResult
            {
                IsError = false,
                Message = $"Созданы перевозки для заказов {orders.Select(x=>x.SalesOrderNumber).Join(", ")}"
            };
        }

        public bool IsAvailable(Role role, IEnumerable<Order> orders)
        {
            return orders.All(x=>x.Status == OrderState.Created) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Application.Actions.Orders
{
    public class SaveOrders : IGroupAppAction<Order>
    {
        private readonly AppDbContext db;

        public SaveOrders(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, IEnumerable<Order> orders)
        {
            var saveOrder = new SaveOrder(db);
            var results = "";
            foreach (var order in orders)
            {
                var appActionResult = saveOrder.Run(user, order);
                results += order;
            }

            return new AppActionResult
            {
                IsError = false,
                Message = $"Заказы {orders.Select(x=>x.SalesOrderNumber).Join(", ")} созданы"
            };
        }

        public bool IsAvailable(Role role, IEnumerable<Order> orders)
        {
            return orders.All(x=>x.Status == OrderState.Draft) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
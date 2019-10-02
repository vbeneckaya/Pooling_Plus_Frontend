using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Microsoft.EntityFrameworkCore.Internal;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Отменить заказы
    /// </summary>
    public class CancelOrders : IGroupAppAction<Order>
    {
        private readonly AppDbContext db;

        public CancelOrders(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Red;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, IEnumerable<Order> orders)
        {
            foreach (var order in orders)
            {
                order.Status = OrderState.Canceled;
            }

            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Заказы {orders.Select(x=>x.OrderNumber).Join(", ")} отменёны"
            };
        }

        public bool IsAvailable(Role role, IEnumerable<Order> orders)
        {
            
            return orders.All(x=>x.Status == OrderState.Created || x.Status == OrderState.Draft)  && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
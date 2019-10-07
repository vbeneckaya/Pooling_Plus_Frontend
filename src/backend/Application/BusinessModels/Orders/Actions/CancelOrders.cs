using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Microsoft.EntityFrameworkCore.Internal;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Отменить заказы
    /// </summary>
    public class CancelOrders : IGroupAppAction<Order>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public CancelOrders(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Red;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, IEnumerable<Order> orders)
        {
            foreach (var order in orders)
            {
                order.Status = OrderState.Canceled;
                _historyService.Save(order.Id, "orderStatusChanged", order.Status);
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
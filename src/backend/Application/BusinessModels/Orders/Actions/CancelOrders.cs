using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
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

        public AppActionResult Run(CurrentUserDto user, IEnumerable<Order> orders)
        {
            string orderNumbers = orders.Select(x => x.OrderNumber).Join(", ");
            foreach (var order in orders)
            {
                order.Status = OrderState.Canceled;
                _historyService.Save(order.Id, "orderSetCancelled", order.OrderNumber);
            }

            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = "ordersSetCancelled".translate(user.Language, orderNumbers)
            };
        }

        public bool IsAvailable(Role role, IEnumerable<Order> orders)
        {
            
            return orders.All(x=>x.Status == OrderState.Created || x.Status == OrderState.Draft)  && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
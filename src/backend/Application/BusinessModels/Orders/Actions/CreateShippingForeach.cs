using System;
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
    /// Создать перевозку для каждого заказа
    /// </summary>
    public class CreateShippingForeach : IGroupAppAction<Order>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public CreateShippingForeach(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, IEnumerable<Order> orders)
        {
            var createShipping = new CreateShipping(db, _historyService);
            foreach (var order in orders) 
                createShipping.Run(user, order);

            return new AppActionResult
            {
                IsError = false,
                Message = $"Созданы перевозки для заказов {orders.Select(x=>x.OrderNumber).Join(", ")}"
            };
        }

        public bool IsAvailable(Role role, IEnumerable<Order> orders)
        {
            return orders.All(x=>x.Status == OrderState.Created) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
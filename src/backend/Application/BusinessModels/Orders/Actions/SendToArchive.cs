using System;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// В архив
    /// </summary>
    public class SendToArchive : IAppAction<Order>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService dataService;

        public SendToArchive(ICommonDataService dataService, IHistoryService historyService)
        {
            this.dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = OrderState.Archive;

            _historyService.Save(order.Id, "orderSetArchived", order.OrderNumber);

            dataService.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Созданна перевозка {order.OrderNumber}"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Delivered && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
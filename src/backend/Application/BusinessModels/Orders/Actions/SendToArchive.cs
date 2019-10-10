using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// В архив
    /// </summary>
    public class SendToArchive : IAppAction<Order>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public SendToArchive(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            order.Status = OrderState.Archive;

            _historyService.Save(order.Id, "orderSetArchived", order.OrderNumber);

            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = "orderSetArchived".translate(user.Language, order.OrderNumber)
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Delivered && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
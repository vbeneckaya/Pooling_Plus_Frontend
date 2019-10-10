using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Создать перевозку
    /// </summary>
    public class CreateShipping : IAppAction<Order>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public CreateShipping(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            var unionOrders = new UnionOrders(db, _historyService);
            return unionOrders.Run(user, new[] { order });
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Created && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
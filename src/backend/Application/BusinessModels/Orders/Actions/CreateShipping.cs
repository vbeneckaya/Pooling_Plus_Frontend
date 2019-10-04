using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Создать перевозку
    /// </summary>
    public class CreateShipping : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public CreateShipping(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            var unionOrders = new UnionOrders(db);
            return unionOrders.Run(user, new[] { order });
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Created && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
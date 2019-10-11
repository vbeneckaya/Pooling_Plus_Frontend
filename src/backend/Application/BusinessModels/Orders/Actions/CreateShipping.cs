using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Создать перевозку
    /// </summary>
    public class CreateShipping : IAppAction<Order>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService dataService;

        public CreateShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            this.dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            var unionOrders = new UnionOrders(dataService, _historyService);
            return unionOrders.Run(user, new[] { order });
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Created && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
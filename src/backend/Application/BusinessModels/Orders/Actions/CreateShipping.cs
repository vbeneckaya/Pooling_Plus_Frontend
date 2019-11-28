using Application.BusinessModels.Shared.Actions;
using DAL.Services;
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
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService _dataService;

        public CreateShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            var unionOrders = new UnionOrders(_dataService, _historyService);
            return unionOrders.Run(user, new[] { order });
        }

        public bool IsAvailable(Order order)
        {
            return order.Status == OrderState.Confirmed;
        }
    }
}
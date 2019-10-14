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
    /// Полный возврат
    /// </summary>
    public class FullReject : IAppAction<Order>
    {
        private readonly ICommonDataService dataService;
        private readonly IHistoryService _historyService;

        public FullReject(ICommonDataService dataService, IHistoryService historyService)
        {
            this.dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Orange;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            order.Status = OrderState.FullReturn;

            _historyService.Save(order.Id, "orderSetFullReturn", order.OrderNumber);

            dataService.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = "orderSetFullReturn".translate(user.Language, order.OrderNumber)
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Shipped && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
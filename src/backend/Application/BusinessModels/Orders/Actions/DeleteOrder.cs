using DAL;
using DAL.Services;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Linq;

namespace Application.BusinessModels.Orders.Actions
{
    public class DeleteOrder : IAppAction<Order>
    {
        private readonly ICommonDataService _dataService;

        public DeleteOrder(ICommonDataService dataService)
        {
            _dataService = dataService;
            Color = AppColor.Red;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            string orderNumber = order.OrderNumber;

            ///TODO: заменить на использованием флага IsActive после реализации фильтров (нужно будет прокидывать условие, что отбираем только активные для грида)
            var itemsDbSet = _dataService.GetDbSet<OrderItem>();
            itemsDbSet.RemoveRange(itemsDbSet.Where(x => x.OrderId == order.Id));

            var historyDbSet = _dataService.GetDbSet<HistoryEntry>();
            historyDbSet.RemoveRange(historyDbSet.Where(x => x.PersistableId == order.Id));

            _dataService.GetDbSet<Order>().Remove(order);

            _dataService.SaveChanges();

            return new AppActionResult
            {
                IsError = false,
                Message = "orderRemoved".translate(user.Language, orderNumber)
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return (order.Status == OrderState.Created || order.Status == OrderState.Draft) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}

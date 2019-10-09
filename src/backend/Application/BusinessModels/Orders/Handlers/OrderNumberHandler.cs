using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.History;

namespace Application.BusinessModels.Orders.Handlers
{
    public class OrderNumberHandler : IFieldHandler<Order, string>
    {
        public void AfterChange(Order order, string oldValue, string newValue)
        {
            OrderType newOrderType;
            if (order.OrderNumber.StartsWith("2"))
                newOrderType = OrderType.Fd;
            else
                newOrderType = OrderType.Or;

            var setter = new FieldSetter<Order>(order, _historyService);
            setter.UpdateField(o => o.OrderType, newOrderType);
            setter.SaveHistoryLog();
        }

        public string ValidateChange(Order order, string oldValue, string newValue)
        {
            return null;
        }

        public OrderNumberHandler(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        private readonly IHistoryService _historyService;
    }
}

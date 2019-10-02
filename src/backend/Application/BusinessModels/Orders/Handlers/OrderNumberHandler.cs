using Application.BusinessModels.Shared.Handlers;
using Domain.Enums;
using Domain.Persistables;

namespace Application.BusinessModels.Orders.Handlers
{
    public class OrderNumberHandler : IFieldHandler<Order, string>
    {
        public void AfterChange(Order order, string oldValue, string newValue)
        {
            if (order.OrderNumber.StartsWith("2"))
                order.OrderType = OrderType.FD;
            else
                order.OrderType = OrderType.OR;
        }

        public string ValidateChange(Order order, string oldValue, string newValue)
        {
            return null;
        }
    }
}

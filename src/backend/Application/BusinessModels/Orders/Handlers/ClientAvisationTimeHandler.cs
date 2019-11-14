using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ClientAvisationTimeHandler : IFieldHandler<Order, TimeSpan?>
    {
        public void AfterChange(Order order, TimeSpan? oldValue, TimeSpan? newValue)
        {
            order.OrderChangeDate = DateTime.Now;
        }

        public string ValidateChange(Order order, TimeSpan? oldValue, TimeSpan? newValue)
        {
            return null;
        }
    }
}

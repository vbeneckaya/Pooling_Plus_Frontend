using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class PickingTypeHandler : IFieldHandler<Order, Guid?>
    {
        public void AfterChange(Order order, Guid? oldValue, Guid? newValue)
        {
            order.OrderChangeDate = DateTime.Now;
        }

        public string ValidateChange(Order order, Guid? oldValue, Guid? newValue)
        {
            return null;
        }
    }
}

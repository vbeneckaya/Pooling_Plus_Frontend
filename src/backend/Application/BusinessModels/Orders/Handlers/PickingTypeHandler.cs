using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class PickingTypeHandler : IFieldHandler<Order, Guid?>
    {
        private readonly bool _isManual;

        public PickingTypeHandler(bool isManual)
        {
            _isManual = isManual;
        }

        public void AfterChange(Order order, Guid? oldValue, Guid? newValue)
        {
            order.OrderChangeDate = DateTime.UtcNow;

            if (_isManual)
            {
                order.ManualPickingTypeId = true;
            }
        }

        public string ValidateChange(Order order, Guid? oldValue, Guid? newValue)
        {
            return null;
        }
    }
}

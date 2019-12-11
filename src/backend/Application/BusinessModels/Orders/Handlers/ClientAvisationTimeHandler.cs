using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ClientAvisationTimeHandler : IFieldHandler<Order, TimeSpan?>
    {
        private readonly bool _isManual;

        public ClientAvisationTimeHandler(bool isManual)
        {
            _isManual = isManual;
        }

        public void AfterChange(Order order, TimeSpan? oldValue, TimeSpan? newValue)
        {
            order.OrderChangeDate = DateTime.UtcNow;
            order.ManualClientAvisationTime = _isManual;
        }

        public string ValidateChange(Order order, TimeSpan? oldValue, TimeSpan? newValue)
        {
            return null;
        }
    }
}

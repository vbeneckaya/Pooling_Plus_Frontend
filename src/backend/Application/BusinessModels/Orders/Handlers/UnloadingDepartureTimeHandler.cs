using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class UnloadingDepartureTimeHandler : IFieldHandler<Order, DateTime?>
    {
        public void AfterChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
        }

        public string ValidateChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.UnloadingArrivalTime.HasValue && newValue.HasValue && order.UnloadingArrivalTime > newValue)
            {
                return $"Время убытия со грузополучателя не может быть раньше Времени прибытия к грузополучателю";
            }
            else
            {
                return null;
            }
        }
    }
}

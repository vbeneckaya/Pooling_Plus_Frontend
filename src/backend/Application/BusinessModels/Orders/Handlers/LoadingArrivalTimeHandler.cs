using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class LoadingArrivalTimeHandler : IFieldHandler<Order, DateTime?>
    {
        public void AfterChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
        }

        public string ValidateChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.LoadingDepartureTime.HasValue && newValue.HasValue && order.LoadingDepartureTime < newValue)
            {
                return $"Время убытия со склада БДФ не может быть раньше Времени прибытия на загрузку (склад БДФ)";
            }
            else
            {
                return null;
            }
        }
    }
}

using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class LoadingDepartureTimeHandler : IFieldHandler<Order, DateTime?>
    {
        public void AfterChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
        }

        public string ValidateChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.LoadingArrivalTime.HasValue && newValue.HasValue && order.LoadingArrivalTime > newValue)
            {
                return $"Время убытия со склада БДФ не может быть раньше Времени прибытия на загрузку (склад БДФ)";
            }
            else if (order.UnloadingArrivalTime.HasValue && newValue.HasValue && order.UnloadingArrivalTime < newValue)
            {
                return $"Время убытия со склада БДФ не может быть позже Времени прибытия к грузополучателю";
            }
            else
            {
                return null;
            }
        }
    }
}

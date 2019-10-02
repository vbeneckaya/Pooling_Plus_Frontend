using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class UnloadingArrivalTimeHandler : IFieldHandler<Order, DateTime?>
    {
        public void AfterChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
        }

        public string ValidateChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.UnloadingDepartureTime.HasValue && newValue.HasValue && order.UnloadingDepartureTime < newValue)
            {
                return $"Время убытия со грузополучателя не может быть раньше Времени прибытия к грузополучателю";
            }
            else if (order.LoadingDepartureTime.HasValue && newValue.HasValue && order.LoadingDepartureTime > newValue)
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

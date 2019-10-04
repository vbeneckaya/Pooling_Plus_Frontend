using Application.BusinessModels.Shared.Handlers;
using DAL;
using Domain.Persistables;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class UnloadingArrivalTimeHandler : IFieldHandler<Order, DateTime?>
    {
        public void AfterChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var ordersToUpdate = _db.Orders.Where(o => o.ShippingId == order.ShippingId
                                                        && o.Id != order.Id
                                                        && o.DeliveryWarehouseId == order.DeliveryWarehouseId)
                                               .ToList();

                foreach (Order updOrder in ordersToUpdate)
                {
                    updOrder.UnloadingArrivalTime = newValue;
                }
            }
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

        public UnloadingArrivalTimeHandler(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}

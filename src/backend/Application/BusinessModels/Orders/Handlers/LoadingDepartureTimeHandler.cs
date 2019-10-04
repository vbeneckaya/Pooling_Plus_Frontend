using Application.BusinessModels.Shared.Handlers;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class LoadingDepartureTimeHandler : IFieldHandler<Order, DateTime?>
    {
        public void AfterChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var departureTimes = _db.Orders.Where(o => o.ShippingId == order.ShippingId && o.Id != order.Id)
                                               .Select(o => o.LoadingDepartureTime)
                                               .ToList();
                departureTimes.Add(newValue);

                var distinctTimes = departureTimes.Distinct().Count();
                if (distinctTimes == 1)
                {
                    var shipping = _db.Shippings.GetById(order.ShippingId.Value);
                    if (shipping != null)
                    {
                        shipping.LoadingDepartureTime = newValue;
                    }
                }
            }
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

        public LoadingDepartureTimeHandler(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}

using Application.BusinessModels.Shared.Handlers;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class LoadingArrivalTimeHandler : IFieldHandler<Order, DateTime?>
    {
        public void AfterChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var ordersToUpdate = _db.Orders.Where(o => o.ShippingId == order.ShippingId
                                                        && o.Id != order.Id
                                                        && o.ShippingWarehouseId == order.ShippingWarehouseId)
                                               .ToList();

                foreach (Order updOrder in ordersToUpdate)
                {
                    updOrder.LoadingArrivalTime = newValue;
                }

                var shipping = _db.Shippings.GetById(order.ShippingId.Value);
                if (shipping != null)
                {
                    shipping.LoadingArrivalTime = newValue;
                }
            }
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

        public LoadingArrivalTimeHandler(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}

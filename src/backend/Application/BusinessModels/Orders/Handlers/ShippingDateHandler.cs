using Application.BusinessModels.Shared.Handlers;
using DAL;
using Domain.Persistables;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ShippingDateHandler : IFieldHandler<Order, DateTime?>
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
                    updOrder.ShippingDate = newValue;
                }
            }
        }

        public string ValidateChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            return null;
        }

        public ShippingDateHandler(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}

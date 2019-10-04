using Application.BusinessModels.Shared.Handlers;
using DAL;
using Domain.Enums;
using Domain.Persistables;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class DeliveryStatusHandler : IFieldHandler<Order, VehicleState>
    {
        public void AfterChange(Order order, VehicleState oldValue, VehicleState newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var ordersToUpdate = _db.Orders.Where(o => o.ShippingId == order.ShippingId
                                                        && o.Id != order.Id
                                                        && o.DeliveryWarehouseId == order.DeliveryWarehouseId)
                                               .ToList();

                foreach (Order updOrder in ordersToUpdate)
                {
                    updOrder.DeliveryStatus = newValue;
                }
            }
        }

        public string ValidateChange(Order order, VehicleState oldValue, VehicleState newValue)
        {
            return null;
        }

        public DeliveryStatusHandler(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}

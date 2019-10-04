using Application.BusinessModels.Shared.Handlers;
using DAL;
using Domain.Enums;
using Domain.Persistables;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ShippingStatusHandler : IFieldHandler<Order, VehicleState>
    {
        public void AfterChange(Order order, VehicleState oldValue, VehicleState newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var ordersToUpdate = _db.Orders.Where(o => o.ShippingId == order.ShippingId
                                                        && o.Id != order.Id
                                                        && o.ShippingWarehouseId == order.ShippingWarehouseId)
                                               .ToList();
                ordersToUpdate.Add(order);

                foreach (Order updOrder in ordersToUpdate)
                {
                    updOrder.ShippingStatus = newValue;

                    if (newValue == VehicleState.VehicleArrived)
                    {
                        updOrder.LoadingArrivalTime = DateTime.Now;
                    }
                    else if (newValue == VehicleState.VehicleDepartured)
                    {
                        updOrder.LoadingArrivalTime = updOrder.LoadingArrivalTime ?? DateTime.Now;
                        updOrder.LoadingDepartureTime = DateTime.Now;
                        updOrder.DeliveryStatus = VehicleState.VehicleWaiting;
                    }
                }
            }
        }

        public string ValidateChange(Order order, VehicleState oldValue, VehicleState newValue)
        {
            return null;
        }

        public ShippingStatusHandler(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}

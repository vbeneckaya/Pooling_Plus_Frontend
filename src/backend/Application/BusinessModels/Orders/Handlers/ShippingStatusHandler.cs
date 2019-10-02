using Application.BusinessModels.Shared.Handlers;
using Domain.Enums;
using Domain.Persistables;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ShippingStatusHandler : IFieldHandler<Order, VehicleState>
    {
        public void AfterChange(Order order, VehicleState oldValue, VehicleState newValue)
        {
            if (newValue == VehicleState.VehicleArrived)
            {
                order.LoadingArrivalTime = DateTime.Now;
            }
            else if (newValue == VehicleState.VehicleDepartured)
            {
                order.LoadingArrivalTime = order.LoadingArrivalTime ?? DateTime.Now;
                order.LoadingDepartureTime = DateTime.Now;
                order.DeliveryStatus = VehicleState.VehicleWaiting;
            }
        }

        public string ValidateChange(Order order, VehicleState oldValue, VehicleState newValue)
        {
            return null;
        }
    }
}

﻿using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ShippingStatusHandler : IFieldHandler<Order, VehicleState>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, VehicleState oldValue, VehicleState newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var ordersToUpdate = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == order.ShippingId
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

        public ShippingStatusHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            this._dataService = dataService;
            _historyService = historyService;
        }

    }
}

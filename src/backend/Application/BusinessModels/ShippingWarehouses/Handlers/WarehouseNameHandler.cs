﻿using Application.BusinessModels.Shared.Handlers;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.ShippingWarehouses.Handlers
{
    public class ShippingWarehouseNameHandler : IFieldHandler<ShippingWarehouse, string>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public ShippingWarehouseNameHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public void AfterChange(ShippingWarehouse entity, string oldValue, string newValue)
        {
            var validStatuses = new[] { OrderState.Draft, OrderState.Created, OrderState.InShipping };
            var orders = _dataService.GetDbSet<Order>()
                                     .Where(x => x.ShippingWarehouseId == entity.Id
                                                && validStatuses.Contains(x.Status)
                                                && (x.ShippingId == null || x.OrderShippingStatus == ShippingState.ShippingCreated))
                                     .ToList();

            foreach (var order in orders)
            {
                _historyService.SaveImpersonated(null, order.Id, "fieldChanged", 
                                                 nameof(order.ShippingWarehouseId).ToLowerFirstLetter(),
                                                 oldValue, newValue);
            }
        }

        public string ValidateChange(ShippingWarehouse entity, string oldValue, string newValue)
        {
            return null;
        }
    }
}

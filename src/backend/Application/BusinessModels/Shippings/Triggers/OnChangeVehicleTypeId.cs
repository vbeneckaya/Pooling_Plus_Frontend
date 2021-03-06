﻿using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;
using DAL.Queries;
using Domain.Extensions;
using Domain.Services.History;
using Domain.Services.Shippings;

namespace Application.BusinessModels.Shippings.Triggers
{
    public class OnChangeVehicleTypeId : ITrigger<Shipping>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly IDeliveryCostCalcService _calcService;        

        public OnChangeVehicleTypeId(ICommonDataService dataService, IHistoryService historyService, IDeliveryCostCalcService calcService)
        {
            _dataService = dataService;
            _historyService = historyService;
            _calcService = calcService;
        }

        public void Execute(Shipping entity)
        {
            var orders = _dataService.GetDbSet<Order>()
                .Where(x => x.ShippingId == entity.Id);

            var vehicleTypes = _dataService.GetDbSet<VehicleType>();
            
            foreach (var orderInShipping in orders)
            {
                if (orderInShipping.VehicleTypeId != entity.VehicleTypeId)
                {
                    VehicleType oldVehicleType = null;
                    VehicleType newVehicleType = null;

                    if (orderInShipping.VehicleTypeId.HasValue)
                        oldVehicleType = vehicleTypes.GetById(orderInShipping.VehicleTypeId.Value);

                    if (entity.VehicleTypeId.HasValue)
                        newVehicleType = vehicleTypes.GetById(entity.VehicleTypeId.Value);

                    orderInShipping.VehicleTypeId = entity.VehicleTypeId;

                    _historyService.Save(orderInShipping.Id, "fieldChangedBy",
                        nameof(orderInShipping.VehicleTypeId).ToLowerFirstLetter(),
                        oldVehicleType, newVehicleType, "onChangeInShipping");
                }
                
                _calcService.UpdateDeliveryCost(entity);

            }
        }

        public bool IsTriggered(EntityChanges<Shipping> changes)
        {
            var watchProperties = new[]
            {
                nameof(Shipping.VehicleTypeId),
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

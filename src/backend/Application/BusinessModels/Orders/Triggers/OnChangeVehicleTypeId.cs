using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;
using Application.Services.Shippings;
using DAL.Queries;
using Domain.Extensions;
using Domain.Services.History;

namespace Application.BusinessModels.Orders.Triggers
{
    public class OnChangeVehicleTypeId : ITrigger<Order>
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

        public void Execute(Order entity)
        {
            if (entity.ShippingId.HasValue)
            {
                var entityShippingId = entity.ShippingId.Value;

                var shipping = _dataService.GetById<Shipping>(entityShippingId);
            
                var orders = _dataService.GetDbSet<Order>()
                    .Where(x => x.ShippingId == entityShippingId);

                var vehicleTypes = _dataService.GetDbSet<VehicleType>();
                
                foreach (var orderInShipping in orders)
                {
                    if (orderInShipping.VehicleTypeId != entity.VehicleTypeId)
                    {
                        VehicleType oldVehicleType = null;
                        VehicleType newVehicleType  = null;
                        
                        if (orderInShipping.VehicleTypeId.HasValue)
                            oldVehicleType = vehicleTypes.GetById(orderInShipping.VehicleTypeId.Value);

                        if (entity.VehicleTypeId.HasValue)
                            newVehicleType = vehicleTypes.GetById(entity.VehicleTypeId.Value);
                        
                        orderInShipping.VehicleTypeId = entity.VehicleTypeId;
                        
                        _historyService.Save(orderInShipping.Id, "fieldChanged",
                            nameof(entity.VehicleTypeId).ToLowerFirstLetter(),
                            oldVehicleType, newVehicleType);
                    }
                }

                shipping.VehicleTypeId = entity.VehicleTypeId;
            }
        }

        public bool IsTriggered(EntityChanges<Order> changes)
        {
            var watchProperties = new[]
            {
                nameof(Order.VehicleTypeId),
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

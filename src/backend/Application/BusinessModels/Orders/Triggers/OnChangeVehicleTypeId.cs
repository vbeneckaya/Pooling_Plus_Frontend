using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Shared;
using System.Linq;
using Application.Shared;

namespace Application.BusinessModels.Orders.Triggers
{
    public class OnChangeVehicleTypeId : ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public OnChangeVehicleTypeId(ICommonDataService dataService, 
            IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public void Execute(Order entity)
        {
            if (entity.ShippingId.HasValue)
            {
                var entityShippingId = entity.ShippingId.Value;

                var shipping = _dataService.GetById<Shipping>(entityShippingId);
            
                var setter = new FieldSetter<Shipping>(shipping, _historyService);
            
                var orders = _dataService.GetDbSet<Order>()
                    .Where(x => x.ShippingId == entityShippingId);

                foreach (var orderInShipping in orders)
                {
                    if (orderInShipping.VehicleTypeId != entity.VehicleTypeId)
                    {
                        var setterOrderInShipping = new FieldSetter<Order>(orderInShipping, _historyService);
                        setterOrderInShipping.UpdateField(s => s.VehicleTypeId, entity.VehicleTypeId);
                    }
                }
            
                if (entity.VehicleTypeId != shipping.VehicleTypeId)
                    setter.UpdateField(s => s.VehicleTypeId, entity.VehicleTypeId);
            }
        }

        public bool IsTriggered(EntityChangesDto<Order> changes)
        {
            var watchProperties = new[]
            {
                nameof(Order.VehicleTypeId),
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

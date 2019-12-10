using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Shared;
using System.Linq;
using Application.Shared;

namespace Application.BusinessModels.Shippings.Triggers
{
    public class OnChangeVehicleTypeId : ITrigger<Shipping>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public OnChangeVehicleTypeId(ICommonDataService dataService, 
            IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public void Execute(Shipping entity)
        {
            var orders = _dataService.GetDbSet<Order>()
                .Where(x => x.ShippingId == entity.Id);

            foreach (var orderInShipping in orders)
            {
                if (orderInShipping.VehicleTypeId != entity.VehicleTypeId)
                {
                    var setterOrderInShipping = new FieldSetter<Order>(orderInShipping, _historyService);
                    setterOrderInShipping.UpdateField(s => s.VehicleTypeId, entity.VehicleTypeId);
                }
            }
        }

        public bool IsTriggered(EntityChangesDto<Shipping> changes)
        {
            var watchProperties = new[]
            {
                nameof(Shipping.VehicleTypeId),
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

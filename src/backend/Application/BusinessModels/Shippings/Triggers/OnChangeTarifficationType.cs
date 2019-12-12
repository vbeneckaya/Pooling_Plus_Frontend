using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;

namespace Application.BusinessModels.Shippings.Triggers
{
    public class OnChangeTarifficationType : ITrigger<Shipping>
    {
        private readonly ICommonDataService _dataService;

        public OnChangeTarifficationType(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        public void Execute(Shipping entity)
        {
            
            var orders = _dataService.GetDbSet<Order>()
                .Where(x => x.ShippingId == entity.Id);

            foreach (var orderInShipping in orders)
            {
                orderInShipping.TarifficationType = entity.TarifficationType;
            }
        }

        public bool IsTriggered(EntityChanges<Shipping> changes)
        {
            var watchProperties = new[]
            {
                nameof(Order.TarifficationType),
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

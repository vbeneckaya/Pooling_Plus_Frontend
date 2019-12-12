using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;

namespace Application.BusinessModels.Orders.Triggers
{
    public class OnChangeTarifficationType : ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;

        public OnChangeTarifficationType(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        public void Execute(Order entity)
        {
            if (entity.ShippingId.HasValue)
            {
                var entityShippingId = entity.ShippingId.Value;

                var shipping = _dataService.GetById<Shipping>(entityShippingId);
            
                var orders = _dataService.GetDbSet<Order>()
                    .Where(x => x.ShippingId == entityShippingId);

                foreach (var orderInShipping in orders)
                {
                    orderInShipping.TarifficationType = entity.TarifficationType;
                }

                shipping.TarifficationType = entity.TarifficationType;
            }
        }

        public bool IsTriggered(EntityChanges<Order> changes)
        {
            var watchProperties = new[]
            {
                nameof(Order.TarifficationType),
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

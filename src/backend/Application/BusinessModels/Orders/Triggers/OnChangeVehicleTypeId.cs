using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;

namespace Application.BusinessModels.Orders.Triggers
{
    public class OnChangeVehicleTypeId : ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;

        public OnChangeVehicleTypeId(ICommonDataService dataService)
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
                    orderInShipping.VehicleTypeId = entity.VehicleTypeId;
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

using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;

namespace Application.BusinessModels.Orders.Triggers
{
    public class OnChangePalletsCountOrDeliveryRegion : ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IShippingTarifficationTypeDeterminer _shippingTarifficationTypeDeterminer;

        public OnChangePalletsCountOrDeliveryRegion(ICommonDataService dataService, 
            IShippingTarifficationTypeDeterminer shippingTarifficationTypeDeterminer)
        {
            _dataService = dataService;
            _shippingTarifficationTypeDeterminer = shippingTarifficationTypeDeterminer;
        }

        public void Execute(Order entity)
        {
            if (entity.ShippingId.HasValue)
            {
                var entityShippingId = entity.ShippingId.Value;

                var shipping = _dataService.GetById<Shipping>(entityShippingId);
            
                var orders = _dataService.GetDbSet<Order>()
                    .Where(x => x.ShippingId == entityShippingId);
            
                var tarifficationType = _shippingTarifficationTypeDeterminer.GetTarifficationTypeForOrders(orders);

                shipping.TarifficationType = tarifficationType;
            }
        }

        public bool IsTriggered(EntityChanges<Order> changes)
        {
            var watchProperties = new[]
            {
                nameof(Order.DeliveryRegion),
                nameof(Order.PalletsCount),
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

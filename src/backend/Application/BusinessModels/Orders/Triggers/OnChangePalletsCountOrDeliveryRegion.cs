using System.Collections.Generic;
using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Shared;
using System.Linq;
using Application.Shared;
using Domain.Enums;

namespace Application.BusinessModels.Orders.Triggers
{
    public class OnChangePalletsCountOrDeliveryRegion : ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly IShippingTarifficationTypeDeterminer _shippingTarifficationTypeDeterminer;

        public OnChangePalletsCountOrDeliveryRegion(ICommonDataService dataService, 
            IHistoryService historyService, 
            IShippingTarifficationTypeDeterminer shippingTarifficationTypeDeterminer)
        {
            _dataService = dataService;
            _historyService = historyService;
            _shippingTarifficationTypeDeterminer = shippingTarifficationTypeDeterminer;
        }

        public void Execute(Order entity)
        {
            var entityShippingId = entity.ShippingId.Value;

            var shipping = _dataService.GetById<Shipping>(entityShippingId);
            
            var setter = new FieldSetter<Shipping>(shipping, _historyService);
            
            var orders = _dataService.GetDbSet<Order>()
                .Where(x => x.ShippingId == entityShippingId);
            
            var tarifficationType = _shippingTarifficationTypeDeterminer.GetTarifficationTypeForOrders(orders);
            
            if (tarifficationType != shipping.TarifficationType)
                setter.UpdateField(s => s.TarifficationType, tarifficationType);
            
        }

        public bool IsTriggered(EntityChangesDto<Order> changes)
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

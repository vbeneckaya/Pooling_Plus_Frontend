using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;
using Application.Services.Shippings;
using Domain.Extensions;
using Domain.Services.History;

namespace Application.BusinessModels.Orders.Triggers
{
    public class OnChangeTarifficationType : ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly IDeliveryCostCalcService _calcService;          

        public OnChangeTarifficationType(ICommonDataService dataService, IHistoryService historyService, IDeliveryCostCalcService calcService)
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

                foreach (var orderInShipping in orders)
                {
                    if (orderInShipping.TarifficationType != entity.TarifficationType)
                    {
                        _historyService.Save(orderInShipping.Id, "fieldChangedBy",
                            nameof(entity.TarifficationType).ToLowerFirstLetter(),
                            orderInShipping.TarifficationType, entity.TarifficationType, "onChangeInOtherOrderInShipping");
                    
                        orderInShipping.TarifficationType = entity.TarifficationType;
                    }
                }

                if (shipping.TarifficationType != entity.TarifficationType)
                {
                    _historyService.Save(shipping.Id, "fieldChangedBy",
                        nameof(shipping.TarifficationType).ToLowerFirstLetter(),
                        shipping.TarifficationType, entity.TarifficationType, "onChangeInIncludedOrder");
                    
                    shipping.TarifficationType = entity.TarifficationType;
                    
                    if(!shipping.ManualTarifficationType)
                        _calcService.UpdateDeliveryCost(shipping);
                }
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

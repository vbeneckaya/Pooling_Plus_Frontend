using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;
using Application.Services.Shippings;
using Domain.Enums;
using Domain.Extensions;
using Domain.Services.History;

namespace Application.BusinessModels.Shippings.Triggers
{
    public class OnChangeTarifficationType : ITrigger<Shipping>
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

        public void Execute(Shipping entity)
        {
            
            var orders = _dataService.GetDbSet<Order>()
                .Where(x => x.ShippingId == entity.Id);

            foreach (var orderInShipping in orders)
            {
                if (orderInShipping.TarifficationType != entity.TarifficationType)
                {
                    _historyService.Save(orderInShipping.Id, "fieldChanged",
                        nameof(entity.TarifficationType).ToLowerFirstLetter(),
                        orderInShipping.TarifficationType, entity.TarifficationType);
                    
                    orderInShipping.TarifficationType = entity.TarifficationType;
                }
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

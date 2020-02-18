using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;
using Domain.Extensions;
using Domain.Services.History;
using Domain.Services.Shippings;

namespace Application.BusinessModels.Shippings.Triggers
{
    public class OnChangeTarifficationType : UpdateIntegratedBase, ITrigger<Shipping>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly IDeliveryCostCalcService _calcService;        

        public OnChangeTarifficationType(ICommonDataService dataService, IHistoryService historyService, IDeliveryCostCalcService calcService)
            :base(dataService)
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
                    _historyService.Save(orderInShipping.Id, "fieldChangedBy",
                        nameof(entity.TarifficationType).ToLowerFirstLetter(),
                        orderInShipping.TarifficationType, entity.TarifficationType, "onChangeInShipping");
                    
                    orderInShipping.TarifficationType = entity.TarifficationType;
                }
            }
            _calcService.UpdateDeliveryCost(entity);
            UpdateShippingFromIntegrations(entity);
        }

        public bool IsTriggered(EntityChanges<Shipping> changes)
        {
            var watchProperties = new[]
            {
                nameof(Shipping.TarifficationType),
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

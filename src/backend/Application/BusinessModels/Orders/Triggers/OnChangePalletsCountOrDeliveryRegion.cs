using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;
using Domain.Enums;
using Domain.Extensions;
using Domain.Services.History;

namespace Application.BusinessModels.Orders.Triggers
{
    public class OnChangePalletsCountOrDeliveryRegion : ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IShippingTarifficationTypeDeterminer _shippingTarifficationTypeDeterminer;
        private readonly IHistoryService _historyService;
        private readonly IDeliveryCostCalcService _calcService;    

        public OnChangePalletsCountOrDeliveryRegion(ICommonDataService dataService, 
            IShippingTarifficationTypeDeterminer shippingTarifficationTypeDeterminer, IHistoryService historyService, 
            IDeliveryCostCalcService calcService)
        {
            _dataService = dataService;
            _shippingTarifficationTypeDeterminer = shippingTarifficationTypeDeterminer;
            _historyService = historyService;
            _calcService = calcService;
        }

        public void Execute(Order entity)
        {
            if (entity.Status == OrderState.Draft || 
                entity.Status == OrderState.Created || 
                entity.Status == OrderState.Confirmed ||
                entity.Status == OrderState.InShipping)
            {
                if (entity.ShippingId.HasValue)
                {
                    var entityShippingId = entity.ShippingId.Value;

                    var shipping = _dataService.GetById<Shipping>(entityShippingId);

                    if (shipping.Status == ShippingState.ShippingCreated)
                    {
                        var orders = _dataService.GetDbSet<Order>()
                            .Where(x => x.ShippingId == entityShippingId);
            
                        var tarifficationType = _shippingTarifficationTypeDeterminer.GetTarifficationTypeForOrders(orders);

                        foreach (var orderInShipping in orders)
                        {
                            if (orderInShipping.TarifficationType != tarifficationType)
                            {
                                _historyService.Save(orderInShipping.Id, "fieldChanged",
                                    nameof(orderInShipping.TarifficationType).ToLowerFirstLetter(),
                                    orderInShipping.TarifficationType, tarifficationType);
                    
                                orderInShipping.TarifficationType = tarifficationType;
                            }
                        }

                        shipping.TarifficationType = tarifficationType;
                
                        if (shipping.TarifficationType != tarifficationType)
                        {
                            _historyService.Save(shipping.Id, "fieldChanged",
                                nameof(shipping.TarifficationType).ToLowerFirstLetter(),
                                shipping.TarifficationType, tarifficationType);
                    
                            shipping.TarifficationType = tarifficationType;
                            _calcService.UpdateDeliveryCost(shipping);
                        }
                    }
                }
                else
                {
                    var tarifficationType = _shippingTarifficationTypeDeterminer.GetTarifficationTypeForOrders(new []{entity});
                    if (entity.TarifficationType != tarifficationType)
                    {
                        _historyService.Save(entity.Id, "fieldChanged",
                            nameof(entity.TarifficationType).ToLowerFirstLetter(),
                            entity.TarifficationType, tarifficationType);
                    
                        entity.TarifficationType = tarifficationType;
                    }
                
                }
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

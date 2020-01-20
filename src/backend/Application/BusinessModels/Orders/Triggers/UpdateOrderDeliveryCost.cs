using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Shared;
using System.Linq;

namespace Application.BusinessModels.Orders.Triggers
{
    public class UpdateOrderDeliveryCost : ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly IDeliveryCostCalcService _calcService;

        public UpdateOrderDeliveryCost(ICommonDataService dataService, IHistoryService historyService, IDeliveryCostCalcService calcService)
        {
            _dataService = dataService;
            _historyService = historyService;
            _calcService = calcService;
        }

        public void Execute(Order entity)
        {
            if (entity.ShippingId != null)
            {
                var shipping = _dataService.GetById<Shipping>(entity.ShippingId.Value);
                _calcService.UpdateDeliveryCost(shipping);
            }
            else if (entity.DeliveryCost != null)
            {
                _historyService.Save(entity.Id, "fieldChanged",
                                     nameof(entity.DeliveryCost).ToLowerFirstLetter(),
                                     entity.DeliveryCost, null);
                entity.DeliveryCost = null;
            }
        }

        public bool IsTriggered(EntityChanges<Order> changes)
        {
            var watchProperties = new[]
            {
                nameof(Order.ShippingId),
                nameof(Order.DeliveryType),
                nameof(Order.PalletsCount),
                nameof(Order.ClientId),
                nameof(Order.ShippingDate),
                nameof(Order.DeliveryDate),
                nameof(Order.ShippingCity),
                nameof(Order.DeliveryCity)
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

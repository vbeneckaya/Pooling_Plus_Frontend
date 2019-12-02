using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;

namespace Application.BusinessModels.Orders.Triggers
{
    public class UpdateOrderDeliveryCost : ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IDeliveryCostCalcService _calcService;

        public UpdateOrderDeliveryCost(ICommonDataService dataService, IDeliveryCostCalcService calcService)
        {
            _dataService = dataService;
            _calcService = calcService;
        }

        public void Execute(Order entity)
        {
            if (entity.ShippingId != null)
            {
                var shipping = _dataService.GetById<Shipping>(entity.ShippingId.Value);
                _calcService.UpdateDeliveryCost(shipping);
            }
        }

        public bool IsTriggered(EntityChangesDto<Order> changes)
        {
            if (changes?.Entity?.ShippingId == null)
            {
                return false;
            }

            var watchProperties = new[]
            {
                nameof(Order.DeliveryType),
                nameof(Order.PalletsCount),
                nameof(Order.ClientName),
                nameof(Order.ShippingDate),
                nameof(Order.DeliveryDate),
                nameof(Order.ShippingCity),
                nameof(Order.DeliveryCity)
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

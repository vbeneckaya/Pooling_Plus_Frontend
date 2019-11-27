using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Persistables;
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

        public bool IsTriggered(Order entity)
        {
            if (entity.ShippingId == null)
            {
                return false;
            }

            var watchProperties = new[]
            {
                nameof(Order.DeliveryType),
                nameof(Order.PalletsCount),
                nameof(Order.ShippingDate),
                nameof(Order.DeliveryDate),
                nameof(Order.ShippingCity),
                nameof(Order.DeliveryCity)
            };
            var trackingEntry = _dataService.GetTrackingEntry(entity);
            var result = trackingEntry.Properties.Any(x => x.IsModified && watchProperties.Contains(x.Metadata.Name));
            return result;
        }
    }
}

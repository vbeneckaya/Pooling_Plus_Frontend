using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Persistables;
using System.Linq;

namespace Application.BusinessModels.Shippings.Triggers
{
    public class UpdateShippingDeliveryCost : ITrigger<Shipping>
    {
        private readonly ICommonDataService _dataService;
        private readonly IDeliveryCostCalcService _calcService;

        public UpdateShippingDeliveryCost(ICommonDataService dataService, IDeliveryCostCalcService calcService)
        {
            _dataService = dataService;
            _calcService = calcService;
        }

        public void Execute(Shipping entity)
        {
            _calcService.UpdateDeliveryCost(entity);
        }

        public bool IsTriggered(Shipping entity)
        {
            var watchProperties = new[]
            {
                nameof(Shipping.CarrierId),
                nameof(Shipping.VehicleTypeId),
                nameof(Shipping.BodyTypeId),
                nameof(Shipping.TarifficationType)
            };
            var trackingEntry = _dataService.GetTrackingEntry(entity);
            var result = trackingEntry.Properties.Any(x => x.IsModified && watchProperties.Contains(x.Metadata.Name));
            return result;
        }
    }
}

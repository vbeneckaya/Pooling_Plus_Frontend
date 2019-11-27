using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using System.Linq;

namespace Application.BusinessModels.Tariffs.Triggers
{
    public class UpdateTariffDeliveryCost : ITrigger<Tariff>
    {
        private readonly ICommonDataService _dataService;
        private readonly IDeliveryCostCalcService _calcService;

        public UpdateTariffDeliveryCost(ICommonDataService dataService, IDeliveryCostCalcService calcService)
        {
            _dataService = dataService;
            _calcService = calcService;
        }

        public void Execute(Tariff entity)
        {
            var shippings = _dataService.GetDbSet<Shipping>().Where(x => x.Status == ShippingState.ShippingCreated).ToList();
            foreach (var shipping in shippings)
            {
                _calcService.UpdateDeliveryCost(shipping);
            }
        }

        public bool IsTriggered(Tariff entity)
        {
            var trackingEntry = _dataService.GetTrackingEntry(entity);
            var result = trackingEntry.Properties.Any(x => x.IsModified);
            return result;
        }
    }
}

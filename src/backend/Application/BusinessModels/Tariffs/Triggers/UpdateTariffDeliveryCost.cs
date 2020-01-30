using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;
using Domain.Services.Shippings;

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
            var shippings = _dataService.GetDbSet<Shipping>()
                                .Where(x => x.Status == ShippingState.ShippingCreated 
                                            || x.Status == ShippingState.ShippingRequestSent 
                                            || x.Status == ShippingState.ShippingRejectedByTc)
                                .ToList();
            foreach (var shipping in shippings)
            {
                _calcService.UpdateDeliveryCost(shipping);
            }
        }

        public bool IsTriggered(EntityChanges<Tariff> changes)
        {
            return changes?.FieldChanges?.Count > 0;
        }
    }
}

using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;
using Domain.Services.Shippings;

namespace Application.BusinessModels.Shippings.Triggers
{
    public class UpdateShippingDeliveryCost : ITrigger<Shipping>
    {
        private readonly IDeliveryCostCalcService _calcService;

        public UpdateShippingDeliveryCost(IDeliveryCostCalcService calcService)
        {
            _calcService = calcService;
        }

        public void Execute(Shipping shipping)
        {
            _calcService.UpdateDeliveryCost(shipping);
        }

        public bool IsTriggered(EntityChanges<Shipping> changes)
        {
            var watchProperties = new[]
            {
                nameof(Shipping.CarrierId),
                nameof(Shipping.VehicleTypeId),
                nameof(Shipping.BodyTypeId),
                nameof(Shipping.TarifficationType)
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

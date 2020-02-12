using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;

namespace Application.BusinessModels.Shippings.Handlers
{
    public class ConfirmedWeightKgHandler : IFieldHandler<Shipping, decimal?>
    {
        public void AfterChange(Shipping shipping, decimal? oldValue, decimal? newValue)
        {
            shipping.ManualConfirmedWeightKg = true;
        }

        public string ValidateChange(Shipping shipping, decimal? oldValue, decimal? newValue)
        {
            return null;
        }
    }
}

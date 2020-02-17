using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;

namespace Application.BusinessModels.Shippings.Handlers
{
    public class TotalDeliveryCostHandler : IFieldHandler<Shipping, decimal?>
    {
        public void AfterChange(Shipping shipping, decimal? oldValue, decimal? newValue)
        {
            shipping.ManualTotalDeliveryCost = true;
        }

        public string ValidateChange(Shipping shipping, decimal? oldValue, decimal? newValue)
        {
            return null;
        }
    }
}

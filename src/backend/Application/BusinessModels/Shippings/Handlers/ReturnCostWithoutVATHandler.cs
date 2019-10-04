using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;

namespace Application.BusinessModels.Shippings.Handlers
{
    public class ReturnCostWithoutVATHandler : IFieldHandler<Shipping, decimal?>
    {
        public void AfterChange(Shipping shipping, decimal? oldValue, decimal? newValue)
        {
            if (!shipping.ManualTotalDeliveryCost)
            {
                shipping.TotalDeliveryCost =
                    shipping.DeliveryCostWithoutVAT ?? 0M
                    + shipping.ReturnCostWithoutVAT ?? 0M
                    + shipping.AdditionalCostsWithoutVAT ?? 0M;
            }
        }

        public string ValidateChange(Shipping shipping, decimal? oldValue, decimal? newValue)
        {
            return null;
        }
    }
}

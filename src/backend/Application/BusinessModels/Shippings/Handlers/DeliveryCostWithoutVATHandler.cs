using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using Domain.Persistables;
using Domain.Services.History;

namespace Application.BusinessModels.Shippings.Handlers
{
    public class DeliveryCostWithoutVATHandler : IFieldHandler<Shipping, decimal?>
    {
        public void AfterChange(Shipping shipping, decimal? oldValue, decimal? newValue)
        {
            if (!shipping.ManualTotalDeliveryCost)
            {
                decimal newTotalValue = shipping.DeliveryCostWithoutVAT ?? 0M + shipping.ReturnCostWithoutVAT ?? 0M + shipping.AdditionalCostsWithoutVAT ?? 0M;
                shipping.TotalDeliveryCost = newTotalValue;
            }
        }

        public string ValidateChange(Shipping shipping, decimal? oldValue, decimal? newValue)
        {
            return null;
        }

        public DeliveryCostWithoutVATHandler(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        private readonly IHistoryService _historyService;
    }
}

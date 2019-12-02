using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using Domain.Persistables;
using Domain.Services.History;

namespace Application.BusinessModels.Shippings.Handlers
{
    public class ReturnCostWithoutVATHandler : IFieldHandler<Shipping, decimal?>
    {
        public void AfterChange(Shipping shipping, decimal? oldValue, decimal? newValue)
        {
            if (!shipping.ManualTotalDeliveryCost)
            {
                var setter = new FieldSetter<Shipping>(shipping);

                decimal newTotalValue = shipping.DeliveryCostWithoutVAT ?? 0M + shipping.ReturnCostWithoutVAT ?? 0M + shipping.AdditionalCostsWithoutVAT ?? 0M;
                setter.UpdateField(s => s.TotalDeliveryCost, newTotalValue);
            }
        }

        public string ValidateChange(Shipping shipping, decimal? oldValue, decimal? newValue)
        {
            return null;
        }

        public ReturnCostWithoutVATHandler(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        private readonly IHistoryService _historyService;
    }
}

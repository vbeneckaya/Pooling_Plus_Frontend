using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;

namespace Application.BusinessModels.Shippings.Handlers
{
    public class ActualPalletsCountHandler : IFieldHandler<Shipping, int?>
    {
        public void AfterChange(Shipping shipping, int? oldValue, int? newValue)
        {
            shipping.ManualActualPalletsCount = true;
        }

        public string ValidateChange(Shipping shipping, int? oldValue, int? newValue)
        {
            return null;
        }
    }
}

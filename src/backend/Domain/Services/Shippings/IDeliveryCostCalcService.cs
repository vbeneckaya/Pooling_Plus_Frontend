using Domain.Persistables;

namespace Domain.Services.Shippings
{
    public interface IDeliveryCostCalcService
    {
        void UpdateDeliveryCost(Shipping shipping);
    }
}
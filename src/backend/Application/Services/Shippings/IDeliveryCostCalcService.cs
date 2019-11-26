using Domain.Persistables;

namespace Application.Services.Shippings
{
    public interface IDeliveryCostCalcService
    {
        void UpdateDeliveryCost(Shipping shipping);
    }
}
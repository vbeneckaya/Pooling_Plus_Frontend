using Domain.Persistables;

namespace Application.Services.Shippings
{
    public interface IDeliveryCostCalcService
    {
        decimal? CalculateDeliveryCost(Shipping shipping);
    }
}
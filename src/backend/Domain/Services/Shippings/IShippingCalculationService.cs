using System.Collections.Generic;
using Domain.Persistables;

namespace Domain.Services.Shippings
{
    public interface IShippingCalculationService
    {
        void RecalculateShipping(Shipping shipping, IEnumerable<Order> orders);
    }
}
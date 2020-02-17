using System.Collections.Generic;
using Domain.Persistables;

namespace Domain.Services.Shippings
{
    public interface IShippingGetRouteService
    {
        void UpdateRoute(Shipping shipping, IEnumerable<Order> orders);
    }
}
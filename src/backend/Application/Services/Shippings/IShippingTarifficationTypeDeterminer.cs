using System.Collections.Generic;
using Domain.Enums;
using Domain.Persistables;

namespace Application.Services.Shippings
{
    public interface IShippingTarifficationTypeDeterminer
    {
        TarifficationType GetTarifficationTypeForOrders(IEnumerable<Order> orders);
    }
}
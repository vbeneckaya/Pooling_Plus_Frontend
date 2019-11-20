using System.Collections.Generic;
using Domain.Services.Warehouses;

namespace Domain.Services.ShippingWarehouses
{
    public interface IShippingWarehousesForOrderCreation
    {
        IEnumerable<ShippingWarehouseDtoForSelect> ForSelect();
    }
}

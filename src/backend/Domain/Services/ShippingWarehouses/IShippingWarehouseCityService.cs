using Domain.Shared;
using System.Collections.Generic;

namespace Domain.Services.ShippingWarehouses
{
    public interface IShippingWarehouseService
    {
        IEnumerable<LookUpDto> ForSelect();
    }
}

using Domain.Shared;
using System.Collections.Generic;

namespace Domain.Services.ShippingWarehouses
{
    public interface IShippingAddressService
    {
        IEnumerable<LookUpDto> ForSelect();
    }
}

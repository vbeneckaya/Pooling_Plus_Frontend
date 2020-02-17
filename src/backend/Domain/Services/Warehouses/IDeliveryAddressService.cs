using Domain.Shared;
using System.Collections.Generic;

namespace Domain.Services.Warehouses
{
    public interface IDeliveryAddressService
    {
        IEnumerable<LookUpDto> ForSelect(string clientId, string deliveryCity);
    }
}

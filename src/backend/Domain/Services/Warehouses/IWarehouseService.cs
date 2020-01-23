using Domain.Shared;
using System.Collections.Generic;

namespace Domain.Services.Warehouses
{
    public interface IWarehouseService
    {
        IEnumerable<LookUpDto> ForSelect();
    }
}

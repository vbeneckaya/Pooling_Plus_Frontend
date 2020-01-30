
using System;
using System.Collections.Generic;
using Domain.Shared;

namespace Domain.Services.Warehouses
{
    public interface IWarehousesService : IDictonaryService<Persistables.Warehouse, WarehouseDto>
    {
        IEnumerable<LookUpDto> ForSelect(Guid clientId);
    }
}
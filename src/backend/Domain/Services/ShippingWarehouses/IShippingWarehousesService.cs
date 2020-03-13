using System;
using System.Collections.Generic;
using Domain.Shared;

namespace Domain.Services.ShippingWarehouses
{
    public interface IShippingWarehousesService : IDictonaryService<Persistables.ShippingWarehouse, ShippingWarehouseDto>
    {
        Persistables.ShippingWarehouse GetByCode(string code);
        
        ShippingWarehouseDto GetByNameAndProviderId(string name, Guid providerId);
        
        Guid? AddColedinoShippingWarehouseToProvider(Guid providerId);
        
    }
}

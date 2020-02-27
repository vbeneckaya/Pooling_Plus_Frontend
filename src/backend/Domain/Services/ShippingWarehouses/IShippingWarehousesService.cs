using System;

namespace Domain.Services.ShippingWarehouses
{
    public interface IShippingWarehousesService : IDictonaryService<Persistables.ShippingWarehouse, ShippingWarehouseDto>
    {
        Persistables.ShippingWarehouse GetByCode(string code);
        
        ShippingWarehouseDto GetByNameAndProviderId(string name, Guid providerId);
    }
}

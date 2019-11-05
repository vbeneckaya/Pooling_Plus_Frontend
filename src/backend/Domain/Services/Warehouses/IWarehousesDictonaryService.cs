using Domain.Persistables;

namespace Domain.Services.Warehouses
{
    public interface IWarehousesService : IDictonaryService<Warehouse, WarehouseDto>
    {
        WarehouseDto GetBySoldTo(string soldToNumber);
    }
}
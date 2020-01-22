namespace Domain.Services.ShippingWarehouses
{
    public interface IShippingWarehousesService : IDictonaryService<Persistables.ShippingWarehouse, ShippingWarehouseDto>
    {
        Persistables.ShippingWarehouse GetByCode(string code);
    }
}

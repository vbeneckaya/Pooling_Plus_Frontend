using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.ShippingWarehouses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/shippingWarehouses")]
    [Route("api/shippingWarehouse")]
    public class ShippingWarehousesController : DictionaryController<IShippingWarehousesService, ShippingWarehouse, ShippingWarehouseDto> 
    {
        public ShippingWarehousesController(IShippingWarehousesService warehousesService) : base(warehousesService)
        {
        }
    }
}
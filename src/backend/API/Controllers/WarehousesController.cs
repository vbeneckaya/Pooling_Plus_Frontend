using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Warehouses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("Warehouses")]
    public class WarehousesController : DictonaryController<IWarehousesService, Warehouse, WarehouseDto> 
    {
        public WarehousesController(IWarehousesService warehousesService) : base(warehousesService)
        {
        }
    }
}
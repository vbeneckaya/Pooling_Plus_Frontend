using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Warehouses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/warehouses")]
    public class WarehousesController : DictionaryController<IWarehousesService, Warehouse, WarehouseDto> 
    {
        public WarehousesController(IWarehousesService warehousesService) : base(warehousesService)
        {
        }
    }
}
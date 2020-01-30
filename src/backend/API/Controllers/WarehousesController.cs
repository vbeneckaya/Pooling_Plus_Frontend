using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Warehouses;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace API.Controllers
{
    [Route("api/warehouses")]
    public class WarehousesController : DictionaryController<IWarehousesService, Warehouse, WarehouseDto> 
    {
        public WarehousesController(IWarehousesService warehousesService) : base(warehousesService)
        {
        }
        
        /// <summary>
        /// Получение данных для выпадающего списка
        /// </summary>
        [HttpGet("byClientId/forSelect")]
        public IEnumerable<LookUpDto> ForSelect(Guid clientId)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var result = _service.ForSelect(clientId).OrderBy(x => x.Name).ToList();
            Log.Information("DeliveryWarehouse.ForSelect: {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);

            return result;
        }
    }
}
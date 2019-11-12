using Domain.Services.WarehouseCity;
using Domain.Services.Warehouses;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/warehouseCity")]
    public class WarehouseCity : Controller
    {
        /// <summary>
        /// Получение данных для выпадающего списка
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect()
        {
            return _service.ForSelect();
        }

        public WarehouseCity(IWarehouseCityService service)
        {
            _service = service;
        }

        private readonly IWarehouseCityService _service;
    }
}

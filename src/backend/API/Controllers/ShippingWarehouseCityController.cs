using Domain.Services.ShippingWarehouseCity;
using Domain.Services.WarehouseCity;
using Domain.Services.Warehouses;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/shippingWarehouseCity")]
    public class ShippingWarehouseCityController : Controller
    {
        /// <summary>
        /// Получение данных для выпадающего списка
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect()
        {
            return _service.ForSelect();
        }

        public ShippingWarehouseCityController(IShippingWarehouseCityService service)
        {
            _service = service;
        }

        private readonly IShippingWarehouseCityService _service;
    }
}

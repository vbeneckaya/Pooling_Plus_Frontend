using System.Collections.Generic;
using Domain.Services.ShippingWarehouses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/shippingWarehousesForOrderCreation")]
    public class ShippingWarehousesForOrderCreationController : Controller
    {
        /// <summary>
        /// Получение данных для выпадающего списка
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<ShippingWarehouseDtoForSelect> ForSelect()
        {
            return _service.ForSelect();
        }

        public ShippingWarehousesForOrderCreationController(IShippingWarehousesForOrderCreation service)
        {
            _service = service;
        }

        private readonly IShippingWarehousesForOrderCreation _service;
    }
}
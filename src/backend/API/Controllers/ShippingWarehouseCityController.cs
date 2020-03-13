//using Domain.Shared;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using Domain.Services.ShippingWarehouses;
//
//namespace API.Controllers
//{
//    [Route("api/shippingWarehouseCity")]
//    public class ShippingWarehouseCityController : Controller
//    {
//        /// <summary>
//        /// Получение данных для выпадающего списка
//        /// </summary>
//        [HttpGet("forSelect")]
//        public IEnumerable<LookUpDto> ForSelect()
//        {
//            return _service.ForSelect();
//        }
//
//        public ShippingWarehouseCityController(IShippingWarehouseService service)
//        {
//            _service = service;
//        }
//
//        private readonly IShippingWarehouseService _service;
//    }
//}

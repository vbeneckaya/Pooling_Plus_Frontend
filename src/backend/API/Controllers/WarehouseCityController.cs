//using Domain.Shared;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using Domain.Services.Warehouses;
//
//namespace API.Controllers
//{
//    [Route("api/warehouseCity")]
//    public class WarehouseCityController : Controller
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
//        public WarehouseCityController(IWarehouseService service)
//        {
//            _service = service;
//        }
//
//        private readonly IWarehouseService _service;
//    }
//}

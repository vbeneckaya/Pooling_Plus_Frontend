using Domain.Services.Warehouses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/soldTo")]
    [ApiExplorerSettings(IgnoreApi=true)]
    public class SoldToController : Controller
    {
        /// <summary>
        /// Получение данных для выпадающего списка
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<SoldToDto> ForSelect()
        {
            return _service.ForSelect();
        }

        public SoldToController(ISoldToService service)
        {
            _service = service;
        }

        private readonly ISoldToService _service;
    }
}

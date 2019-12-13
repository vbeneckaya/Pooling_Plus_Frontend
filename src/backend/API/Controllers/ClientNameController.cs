using Domain.Services.Warehouses;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/clientName")]
    public class ClientNameController : Controller
    {
        /// <summary>
        /// Получение данных для выпадающего списка
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect()
        {
            return _service.ForSelect();
        }

        public ClientNameController(IClientNameService service)
        {
            _service = service;
        }

        private readonly IClientNameService _service;
    }
}

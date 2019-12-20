using Domain.Services.ShippingWarehouses;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace API.Controllers
{
    /// <summary>
    /// Точки отгрузки
    /// </summary>
    [Route("api/shippingAddress")]
    public class ShippingAddressController : Controller
    {
        protected readonly IShippingAddressService _service;

        public ShippingAddressController(IShippingAddressService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получение данных для выпадающего списка
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var result = _service.ForSelect().OrderBy(x => x.Name).ToList();
            Log.Information("ShippingAddress.ForSelect: {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);

            return result;
        }
    }
}

using Domain.Services.Warehouses;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace API.Controllers
{
    /// <summary>
    /// Точки доставки
    /// </summary>
    [Route("api/deliveryAddress")]
    public class DeliveryAddressController : Controller
    {
        protected readonly IDeliveryAddressService _service;

        public DeliveryAddressController(IDeliveryAddressService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получение данных для выпадающего списка
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect(string clientId = null, string deliveryCity = null)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var result = _service.ForSelect(clientId, deliveryCity).OrderBy(x => x.Name).ToList();
            Log.Information("DeliveryAddress.ForSelect: {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);

            return result;
        }
    }
}

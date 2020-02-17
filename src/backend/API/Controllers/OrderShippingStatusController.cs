using Domain.Services.Orders;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/orderShippingStatus")]
    public class OrderShippingStatusController : Controller
    {
        private readonly IOrderShippingStatusService _stateService;

        public OrderShippingStatusController(IOrderShippingStatusService stateService)
        {
            _stateService = stateService;
        }

        /// <summary>
        /// ��� ��������� ������� � �������
        /// </summary>
        [HttpPost("search")]
        public IEnumerable<StateDto> GetAll()
        {
            var result = _stateService.GetAll();
            return result;
        }

        /// <summary>
        /// ��� ��������� �������
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect()
        {
            var result = _stateService.ForSelect();
            return result;
        }
    }
}
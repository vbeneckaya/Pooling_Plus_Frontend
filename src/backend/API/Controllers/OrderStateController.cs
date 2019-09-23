using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Enums;
using Domain.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/orderState")]
    public class OrderStateController :StateController<OrderState>
    {
        
    }
    
    [Route("api/shippingState")]
    public class ShippingStateController :StateController<ShippingState>
    {
        
    }

    public class StateController<T> :  Controller
    {
        /// <summary>
        /// Все доступные статусы с цветами
        /// </summary>
        [HttpPost("search")]
        public IEnumerable<StateDto> GetAll()
        {
            var values = Enum.GetValues(typeof(T));
            var result = new List<StateDto>();
            foreach (var value in values)
            {
                result.Add(new StateDto
                {
                    Name = value.ToString(),
                    Color = AppColor.Blue.ToString()                    
                });
            }

            return result;
        }        
    }

    public class StateDto
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
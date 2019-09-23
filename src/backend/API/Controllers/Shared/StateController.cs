using System;
using System.Collections.Generic;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
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
}
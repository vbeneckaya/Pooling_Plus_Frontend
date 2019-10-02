using System;
using System.Collections.Generic;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class EnumController<T> :  Controller
    {
        /// <summary>
        /// Все доступные статусы
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect()
        {
            var values = Enum.GetValues(typeof(T));
            var result = new List<LookUpDto>();
            foreach (var value in values)
            {
                result.Add(new LookUpDto
                {
                    Name = value.ToString(), 
                    Value = value.ToString()
                });
            }

            return result;
        }
    }
}
using Domain.Extensions;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
            var values = Extensions.GetOrderedEnum<T>();
            var result = new List<LookUpDto>();
            foreach (var value in values)
            {
                string name = value.ToString().ToLowerFirstLetter();
                result.Add(new LookUpDto
                {
                    Name = name, 
                    Value = name
                });
            }

            return result;
        }
    }
}
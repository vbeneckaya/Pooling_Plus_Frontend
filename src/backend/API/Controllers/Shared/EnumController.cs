using System;
using System.Collections.Generic;
using Domain.Extensions;
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
                string name = value.ToString().ToLowerfirstLetter();
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
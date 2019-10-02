using System;
using System.Collections.Generic;
using Domain.Extensions;
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
                    Name = value.ToString().ToLowerfirstLetter(),
                    Color = value.GetColor().ToString().ToLowerfirstLetter()                    
                });
            }

            return result;
        }

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
                    Name = value.ToString().ToLowerfirstLetter(), 
                    Value = value.ToString()
                });
            }

            return result;
        }
    }
}
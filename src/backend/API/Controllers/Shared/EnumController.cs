using System.Collections.Generic;
using System.Linq;
using Domain.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public abstract class EnumController<TService, TEnum> :  Controller where TService : IEnumService<TEnum>
    {
        protected readonly TService service;
        
        public EnumController(TService service)
        {
            this.service = service;
        }
        
        /// <summary>
        /// Получение данных для выпадающего списка в 
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect()
        {
            return service.ForSelect().OrderBy(x => x.Name);
        }

    }
}
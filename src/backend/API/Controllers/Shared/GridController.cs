using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Services;
using Infrastructure;
using Infrastructure.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Shared
{
    public abstract class GridController<TService, TEntity, TDto> : Controller where TService : IGridService<TEntity, TDto>
    {
        protected readonly TService service;

        public GridController(TService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Поиск по вхождению с пагинацией
        /// </summary>
        [HttpPost("search")]
        public IEnumerable<TDto> Search([FromBody]SearchForm form)
        {
            return service.Search(form);
        }

        /// <summary>
        /// Данные по id
        /// </summary>
        [HttpGet("getById/{id}")]
        public TDto GetById(Guid id)
        {
            var user = service.Get(id);
            return user;
        }
        
        /// <summary>
        /// Список возможных экшенов
        /// </summary>
        [HttpPost("getActions")]
        public IEnumerable<ActionDto> GetActions([FromBody]IEnumerable<string> ids)
        {
            return service.GetActions(ids.Select(Guid.Parse));
        }
        
        /// <summary>
        /// Выполнить действие
        /// </summary>
        [HttpPost("invokeAction/{name}")]
        public IEnumerable<ActionDto> InvokeAction(string name, [FromBody]IEnumerable<string> ids)
        {
            return new List<ActionDto>
            {
                new ActionDto
                {
                    Ids = ids,
                    Name = "Test",
                    Color = "blue"
                }
            };
        }
        
        
        /// <summary>
        /// Сохранить или изменить
        /// </summary>
        [HttpPost("saveOrCreate")]
        public ValidateResult SaveOrCreate([FromBody] TDto form)
        {
            return service.SaveOrCreate(form);
        }
    }
}
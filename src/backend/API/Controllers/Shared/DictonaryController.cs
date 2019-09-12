using System;
using System.Collections.Generic;
using Domain.Services;
using Infrastructure.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Shared
{
    public abstract class DictonaryController<TService, TEntity, TDto> : Controller where TService: IDictonaryService<TEntity, TDto>
    {
        protected readonly TService service;

        public DictonaryController(TService service)
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
        /// Сохранить или изменить
        /// </summary>
        [HttpPost("saveOrCreate")]
        public ValidateResult SaveOrCreate([FromBody] TDto form)
        {
            return service.SaveOrCreate(form);
        }
    }
}
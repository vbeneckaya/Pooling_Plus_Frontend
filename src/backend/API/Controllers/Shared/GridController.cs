using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
        public IActionResult Search([FromBody]SearchForm form)
        {
            try
            {
                IEnumerable<TDto> result = service.Search(form);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Search {typeof(TEntity).Name}");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Данные по id
        /// </summary>
        [HttpGet("getById/{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                TDto user = service.Get(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Get {typeof(TEntity).Name} by {id}");
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Список возможных экшенов
        /// </summary>
        [HttpPost("getActions")]
        public IActionResult GetActions([FromBody]IEnumerable<string> ids)
        {
            try
            {
                IEnumerable<ActionDto> result = service.GetActions(ids.Select(Guid.Parse));
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Get actions for {typeof(TEntity).Name}");
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Выполнить действие
        /// </summary>
        [HttpPost("invokeAction/{name}")]
        public IActionResult InvokeAction(string name, [FromBody]IEnumerable<string> ids)
        {
            try
            {
                AppActionResult result = service.InvokeAction(name, ids.Select(Guid.Parse));
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Invoke action {name} for {typeof(TEntity).Name}");
                return StatusCode(500, ex.Message);
            }
        }
        
        
        /// <summary>
        /// Сохранить или изменить
        /// </summary>
        [HttpPost("saveOrCreate")]
        public IActionResult SaveOrCreate([FromBody] TDto form)
        {
            try
            {
                ValidateResult result = service.SaveOrCreate(form);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Save or create {typeof(TEntity).Name}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
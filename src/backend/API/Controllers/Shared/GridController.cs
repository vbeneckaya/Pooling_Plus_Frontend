using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace API.Controllers.Shared
{
    public abstract class GridController<TService, TEntity, TDto, TFormDto> : Controller where TService : IGridService<TEntity, TDto, TFormDto>
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
                var result = service.Search(form);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Search {typeof(TEntity).Name}");
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Получение данных для выпадающего списка в 
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect()
        {
            return service.ForSelect();
        }
        
        /// <summary>
        /// Импортировать
        /// </summary>
        [HttpPost("import")]
        public IEnumerable<ValidateResult> Import([FromBody] IEnumerable<TFormDto> form)
        {
            return service.Import(form);
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
        /// Импортировать из excel
        /// </summary>
        [HttpPost("importFromExcel"), DisableRequestSizeLimit]
        public ValidateResult ImportFromExcel()
        {
            var file = HttpContext.Request.Form.Files.ElementAt(0);
            return service.ImportFromExcel(file.OpenReadStream());            
        }      
        
        
        //GET api/download/12345abc
        [HttpGet("exportToExcel")]
        public IActionResult ExportToExcel() {
            Stream stream = System.IO.File.Open("e:/work_repo/alternative-tms/RunAllWitchWatch.ps1", FileMode.Open);

            if(stream == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(stream, "application/octet-stream", "Экспорт 26-09-19.excel"); // returns a FileStreamResult
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
        public IActionResult SaveOrCreate([FromBody] TFormDto form)
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
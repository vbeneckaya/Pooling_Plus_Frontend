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
    public abstract class GridController<TService, TEntity, TDto, TFormDto, TSummaryDto> : Controller 
        where TService : IGridService<TEntity, TDto, TFormDto, TSummaryDto>
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
        public IActionResult Search([FromBody]TSearchForm form)
        {
            try
            {
                var result = service.Search(form);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Search {typeof(TDto).Name}");
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
        /// Данные по id
        /// </summary>
        [HttpGet("getById/{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                TFormDto entity = service.GetForm(id);
                return Ok(entity);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Get {typeof(TDto).Name} by {id}");
                return StatusCode(500, ex.Message);
            }
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
        /// Импортировать из excel
        /// </summary>
        [HttpPost("importFromExcel"), DisableRequestSizeLimit]
        public IEnumerable<ValidateResult> ImportFromExcel()
        {
            var file = HttpContext.Request.Form.Files.ElementAt(0);
            return service.ImportFromExcel(file.OpenReadStream());            
        }      
        
        
        /// <summary>
        /// Экспортировать в excel
        /// </summary>
        [HttpGet("exportToExcel"), DisableRequestSizeLimit]
        public IActionResult ExportToExcel() {
            
            var memoryStream = service.ExportToExcel();
            return File(memoryStream, "application/vnd.ms-excel", "exportOrders-26.09.19.xlsx");

            //var memoryStream = new MemoryStream();

            //var stream = service.ExportToExcel();
            //stream.CopyTo(memoryStream);

            //return new FileContentResult(memoryStream.ToArray(), "application/octet-stream");
            //return File(stream, "application/vnd.ms-excel", "exportOrders-26.09.19.xlsx");
        }

        /// <summary>
        /// Получение сводной информации по выделенным записям
        /// </summary>
        [HttpPost("getSummary")]
        public IActionResult GetSummary([FromBody]IEnumerable<string> ids)
        {
            try
            {
                TSummaryDto result = service.GetSummary(ids.Select(Guid.Parse));
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Get summary for {typeof(TEntity).Name}");
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
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Get actions for {typeof(TDto).Name}");
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
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Invoke action {name} for {typeof(TDto).Name}");
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
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(); 
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Save or create {typeof(TDto).Name}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
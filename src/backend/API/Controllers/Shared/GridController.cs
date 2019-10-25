using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.Enums;
using Domain.Extensions;
using Domain.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace API.Controllers.Shared
{
    public abstract class GridController<TService, TEntity, TDto, TFormDto, TSummaryDto, TFilter> : Controller 
        where TService : IGridService<TEntity, TDto, TFormDto, TSummaryDto, TFilter>
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
        public IActionResult Search([FromBody]FilterFormDto<TFilter> form)
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
                Log.Error(ex, $"Failed to Search {EntityName}");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Получение Id сущностей, подходящих под фильтр
        /// </summary>
        [HttpPost("ids")]
        public IActionResult SearchIds([FromBody]FilterFormDto<TFilter> form)
        {
            try
            {
                var result = service.SearchIds(form);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Load IDs of {typeof(TEntity).Name}");
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
                Log.Error(ex, $"Failed to Get {EntityName} by {id}");
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
        public ValidateResult ImportFromExcel()
        {
            var file = HttpContext.Request.Form.Files.ElementAt(0);
            return service.ImportFromExcel(file.OpenReadStream());            
        }

        /// <summary>
        /// Экспортировать в excel
        /// </summary>
        [HttpPost("exportToExcel"), DisableRequestSizeLimit]
        public IActionResult ExportToExcel([FromBody]ExportExcelFormDto<TFilter> dto) {
            
            var memoryStream = service.ExportToExcel(dto);
            return File(memoryStream, "application/vnd.ms-excel", $"Export {EntityName.Pluralize()} {DateTime.Now.ToString("dd.MM.yy HH.mm")}.xlsx");
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
                Log.Error(ex, $"Failed to Get actions for {EntityName}");
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
                Log.Error(ex, $"Failed to Invoke action {name} for {EntityName}");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Список возможных массовых изменений
        /// </summary>
        [HttpPost("getBulkUpdates")]
        public IActionResult GetBulkUpdates([FromBody]IEnumerable<string> ids)
        {
            try
            {
                IEnumerable<BulkUpdateDto> result = service.GetBulkUpdates(ids.Select(Guid.Parse));
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Get bulk updates for {EntityName}");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Применить массовое изменения
        /// </summary>
        [HttpPost("invokeBulkUpdate/{name}")]
        public IActionResult InvokeBulkUpdate(string name, [FromBody]BulkUpdateFormDto dto)
        {
            try
            {
                AppActionResult result = service.InvokeBulkUpdate(name, dto.Ids.Select(Guid.Parse), dto.Value);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Invoke bulk update {name} for {EntityName}");
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
                Log.Error(ex, $"Failed to Save or create {EntityName}");
                return StatusCode(500, ex.Message);
            }
        }

        private string EntityName
        {
            get
            {
                string dtoSuffix = "Dto";
                string name = typeof(TDto).Name;
                if (name.EndsWith(dtoSuffix))
                {
                    name = name.Substring(0, name.Length - dtoSuffix.Length);
                }
                return name;
            }
        }
    }
}
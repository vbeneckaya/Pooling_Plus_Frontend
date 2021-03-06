using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Domain.Extensions;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace API.Controllers.Shared
{
    /// <summary>
    /// Словарь
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDto"></typeparam>
    public abstract class DictionaryController<TService, TEntity, TDto> : Controller
        where TService : IDictonaryService<TEntity, TDto>
    {
        protected readonly TService _service;

        /// <summary>
        /// Словарь
        /// </summary>
        /// <param name="service"></param>
        public DictionaryController(TService service)
        {
            _service = service;
        }

        /// <summary>
        /// Поиск по вхождению с пагинацией
        /// </summary>
        [HttpPost("search")]
        public SearchResult<TDto> Search([FromBody] SearchFormDto form)
        {
            return _service.Search(form);
        }

        /// <summary>
        /// Получение данных для выпадающего списка в 
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect(Guid? filter)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var result = _service.ForSelect(filter).OrderBy(x => x.Name).ToList();
            
            Log.Information("{entityName}.ForSelect: {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }

        /// <summary>
        /// Данные по id
        /// </summary>
        [HttpGet("getById/{id}")]
        public TDto GetById(Guid id)
        {
            var user = _service.Get(id);
            return user;
        }

        /// <summary>
        /// Импортировать
        /// </summary>
        [HttpPost("import")]
        public ImportResultDto Import([FromBody] IEnumerable<TDto> form)
        {
            return _service.Import(form);
        }

        /// <summary>
        /// Импортировать из excel
        /// </summary>
        [HttpPost("importFromExcel")]
        public ImportResultDto ImportFromExcel()
        {
            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            using (var stream = new FileStream(Path.GetTempFileName(), FileMode.Create))
            {
                file.CopyTo(stream);
                return _service.ImportFromExcel(stream);
            }
        }

        /// <summary>
        /// Экспортировать в excel
        /// </summary>
        /// <response code="200">Returns the requested file</response>
        [HttpPost("exportToExcel"), DisableRequestSizeLimit]
        public IActionResult ExportToExcel([FromBody] SearchFormDto form)
        {
            var memoryStream = _service.ExportToExcel(form);
            return File(memoryStream, "application/vnd.ms-excel",
                $"Export {EntityName.Pluralize()} {DateTime.Now.ToString("dd.MM.yy HH.mm")}.xlsx");
        }

        /// <summary>
        /// Сохранить или изменить
        /// </summary>
        [HttpPost("saveOrCreate")]
        public ValidateResult SaveOrCreate([FromBody] TDto form)
        {
            return _service.SaveOrCreate(form);
        }

        /// <summary>
        /// Удалить
        /// </summary>
        [HttpDelete("delete")]
        public ValidateResult Delete(Guid id)
        {
            return _service.Delete(id);
        }

        private string EntityName => typeof(TEntity).Name;
    }
}
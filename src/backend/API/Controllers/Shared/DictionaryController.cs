using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Shared
{
    /// <summary>
    /// Словарь
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDto"></typeparam>
    public abstract class DictionaryController<TService, TEntity, TDto> : Controller where TService: IDictonaryService<TEntity, TDto>
    {
        private readonly TService _service;

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
        public SearchResult<TDto> Search([FromBody]SearchForm form)
        {
            return _service.Search(form);
        }

        /// <summary>
        /// Получение данных для выпадающего списка в 
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect()
        {
            return _service.ForSelect();
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
        public IEnumerable<ValidateResult> Import([FromBody] IEnumerable<TDto> form)
        {
            return _service.Import(form);
        }

        /// <summary>
        /// Импортировать из excel
        /// </summary>
        [HttpPost("importFromExcel")]
        public IEnumerable<ValidateResult> ImportFromExcel()
        {
            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            using (var stream = new FileStream(Path.GetTempFileName(), FileMode.Create))
            {
                file.CopyTo(stream);
                return _service.ImportFromExcel(stream);
            }
            
        }

        /// <summary>
        /// Сохранить или изменить
        /// </summary>
        [HttpPost("saveOrCreate")]
        public ValidateResult SaveOrCreate([FromBody] TDto form)
        {
            return _service.SaveOrCreate(form);
        }
    }
}
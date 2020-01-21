using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Domain.Services.FieldProperties;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    /// <summary>
    /// Настройка полей
    /// </summary>    
    [Route("api/fieldProperties")]
    public class FieldPropertiesController : Controller
    {
        private readonly IFieldPropertiesService fieldPropertiesService;

        public FieldPropertiesController(IFieldPropertiesService fieldPropertiesService)
        {
            this.fieldPropertiesService = fieldPropertiesService;
        }

        /// <summary>
        /// Получить список полей и отображения по статусам
        /// </summary>
        [HttpPost("get")]
        public IEnumerable<FieldForFieldProperties> GetFor([FromBody] FieldPropertiesGetForParams getForParams)
        {
            var companyId = string.IsNullOrEmpty(getForParams.CompanyId)
                ? (Guid?)null
                : Guid.Parse(getForParams.CompanyId);
            
            var roleId = string.IsNullOrEmpty(getForParams.RoleId)
                ? (Guid?)null
                : Guid.Parse(getForParams.RoleId);

            return fieldPropertiesService.GetFor(getForParams.ForEntity, companyId, roleId, null);
        }

        /// <summary>
        /// Получить права доступа для поля
        /// </summary>
        [HttpPost("getField")]
        public IActionResult GetForField([FromBody] GetForFieldPropertyParams dto)
        {
            var accessType = fieldPropertiesService.GetAccessTypeForField(dto);
            return Ok(new { accessType });
        }

        /// <summary>
        /// Сохранить
        /// </summary>
        [HttpPost("save")]
        public ValidateResult Save([FromBody] FieldPropertyDto fieldPropertiesDto)
        {
            return fieldPropertiesService.Save(fieldPropertiesDto);
        }        

        /// <summary>
        /// Переключить "Скрыто" у поля
        /// </summary>
        [HttpPost("toggleHiddenState")]
        public ValidateResult ToggleHiddenState([FromBody] ToggleHiddenStateDto dto)
        {
            return fieldPropertiesService.ToggleHiddenState(dto);
        }

        /// <summary>
        /// Экспортировать в файл в формате JSON
        /// </summary>
        [HttpPost("export/{entity}")]
        public IActionResult Export([FromRoute] string entity, [FromBody] IEnumerable<FieldForFieldProperties> data)
        {
            return File(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)), "text/plain",
                $"Export {entity} FieldProperties {DateTime.Now.ToString("dd.MM.yy HH.mm")}.txt");
        }

        /// <summary>
        /// Импорт из файла (JSON)
        /// </summary>
        [HttpPost("import/{entity}/{roleId}")]
        public IActionResult Import([FromRoute] string entity, [FromRoute] string roleId)
        {
            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            using (var stream = new FileStream(Path.GetTempFileName(), FileMode.Create))
            {
                file.CopyTo(stream);
                if (fieldPropertiesService.Import(stream, new FieldPropertiesGetForParams()
                {
                    ForEntity = entity,
                    RoleId = roleId,
                    CompanyId = null
                }))
                    return Ok();
                return BadRequest();
            }
        }
    }
}
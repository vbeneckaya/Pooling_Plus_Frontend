using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.Services.FieldProperties;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Настройка полей
    /// </summary>    
    [Route("api/fieldProperties")]
    [ApiExplorerSettings(IgnoreApi=true)]
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
            var roleId = string.IsNullOrEmpty(getForParams.RoleId)
                ? (Guid?)null
                : Guid.Parse(getForParams.RoleId);

            return fieldPropertiesService.GetFor(getForParams.ForEntity, roleId, null);
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
        [HttpPost("export/{entity}/{roleId}")]
        public IActionResult Export([FromRoute] string entity, [FromRoute] string roleId, [FromBody] IEnumerable<FieldForFieldProperties> data)
        {
            return File(fieldPropertiesService.Export(data), "text/plain",
                $"Export {entity} for {roleId} FieldProperties {DateTime.Now.ToString("dd.MM.yy HH.mm")}.txt");
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
                    RoleId = roleId
                }))
                    return Ok();
                return BadRequest();
            }
        }
    }
}
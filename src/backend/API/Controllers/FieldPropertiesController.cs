using System;
using System.Collections.Generic;
using Domain.Services.FieldProperties;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

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
    }
}
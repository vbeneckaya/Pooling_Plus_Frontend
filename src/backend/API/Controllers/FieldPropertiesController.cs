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
        /// Получить возможные значения селекторов
        /// </summary>
        public FieldPropertiesSelectors GetSelectors([FromBody] FieldPropertiesGetForParams getForParams)
        {
            var companyId = getForParams.CompanyId == "all"
                ? (Guid?)null
                : Guid.Parse(getForParams.CompanyId);
            
            var roleId = getForParams.RoleId == "all"
                ? (Guid?)null
                : Guid.Parse(getForParams.RoleId);

            return fieldPropertiesService.GetSelectors(companyId, roleId);
        }

        /// <summary>
        /// Получить список полей и отображения по статусам
        /// </summary>
        public IEnumerable<FieldForFieldProperties> GetFor([FromBody] FieldPropertiesGetForParams getForParams)
        {
            var companyId = getForParams.CompanyId == "all"
                ? (Guid?)null
                : Guid.Parse(getForParams.CompanyId);
            
            var roleId = getForParams.RoleId == "all"
                ? (Guid?)null
                : Guid.Parse(getForParams.RoleId);

            return fieldPropertiesService.GetFor(getForParams.ForEntity, companyId, roleId, null);
        }

        /// <summary>
        /// Сохранить
        /// </summary>
        public ValidateResult Save([FromBody] FieldPropertiesDto fieldPropertiesDto)
        {
            return fieldPropertiesService.Save(fieldPropertiesDto);
        }
    }
}
using System;
using System.Linq;
using System.Text;
using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Translations;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace API.Controllers
{
    [Route("api/translations")]
    public class TranslationsController : DictionaryController<ITranslationsService, Translation, TranslationDto>
    {
        public TranslationsController(ITranslationsService service) : base(service)
        {
        }
    }
    
    [Route("api/translation")]
    public class TranslationController : Controller
    {
        private readonly ITranslationsService service;

        public TranslationController(ITranslationsService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Получение переводов для языка
        /// </summary>
        [HttpGet("GetForLangType/{langType}")]        
        public IActionResult GetForLangType(string langType)
        {
            try
            {
                var translationDtos = service.GetAll().ToList();
                var result = "{\n";
                foreach (var translationDto in translationDtos)
                {
                    var value = langType == "en" ? translationDto.En : translationDto.Ru;
                    result += "\""+translationDto.Name+"\": \""+value+"\"" + (translationDto.Name != translationDtos.Last().Name ? ",\n" : "\n");
                }
                result += "}";
            
                return File(Encoding.UTF8.GetBytes(result), "application/octet-stream");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to Get translations for language");
                return StatusCode(500, ex.Message);
            }
        }
    }    
}
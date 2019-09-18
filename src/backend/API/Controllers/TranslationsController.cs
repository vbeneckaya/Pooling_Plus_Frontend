using System.Linq;
using System.Text;
using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Translations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/translations")]
    public class TranslationsController : DictonaryController<ITranslationsService, Translation, TranslationDto>
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
    }    
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Persistables;
using Domain.Services.Translations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("Translations")]
    public class TranslationsController : DictonaryController<ITranslationsService, Translation, TranslationDto>
    {
        public TranslationsController(ITranslationsService translationsService) : base(translationsService)
        {
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
                var value = langType == "En" ? translationDto.En : translationDto.Ru;
                result += "\""+translationDto.Name+"\": \""+value+"\"" + (translationDto.Name != translationDtos.Last().Name ? ",\n" : "\n");
            }
            result += "}";
            
            return File(Encoding.UTF8.GetBytes(result), "application/octet-stream");
        }
    }
}
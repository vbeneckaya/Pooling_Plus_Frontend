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
    }
}
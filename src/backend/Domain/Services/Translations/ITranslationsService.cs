using System.Collections.Generic;

namespace Domain.Services.Translations
{
    public interface ITranslationsService : IService
    {
        IEnumerable<TranslationDto> GetAll();
    }
}
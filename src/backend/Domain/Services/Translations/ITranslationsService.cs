using System.Collections.Generic;
using Domain.Persistables;

namespace Domain.Services.Translations
{
    public interface ITranslationsService : IDictonaryService<Translation, TranslationDto>
    {
        IEnumerable<TranslationDto> GetAll();
    }
}
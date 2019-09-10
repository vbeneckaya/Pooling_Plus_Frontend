using Domain.Persistables;
using Domain.Services.Users;

namespace Domain.Services.Translations
{
    public interface ITranslationsService : IDictonaryService<Translation, TranslationDto>
    {
    }
}
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain.Services.Translations;

namespace Application.Services.Translations
{
    public class TranslationsService : ITranslationsService
    {
        private readonly AppDbContext db;

        public TranslationsService(AppDbContext db)
        {
            this.db = db;
        }

        public IEnumerable<TranslationDto> GetAll()
        {
            return db.Translations.ToList().Select(x=>
            {
                return new TranslationDto
                {
                    Name = x.Name,
                    Ru = x.Ru,
                    En = x.En
                };
            } );
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Translations;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Translations
{
    public class TranslationsService : DictonaryServiceBase<Translation, TranslationDto>, ITranslationsService
    {
        public TranslationsService(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override DbSet<Translation> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Translations;
        }

        public override void MapFromDtoToEntity(Translation entity, TranslationDto dto)
        {
            entity.Name = dto.Name;
            entity.Ru = dto.Ru;
            entity.En = dto.En;
        }

        public override TranslationDto MapFromEntityToDto(Translation entity)
        {
            return new TranslationDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                Ru = entity.Ru,
                En = entity.En
            };
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
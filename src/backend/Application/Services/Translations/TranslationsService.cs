using System;
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
        
        public IEnumerable<TranslationDto> GetAll()
        {
            return db.Translations.ToList().Select(x=>
            {
                return new TranslationDto
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Ru = x.Ru,
                    En = x.En
                };
            } );
        }

        public override DbSet<Translation> UseDbSet(AppDbContext dbContext)
        {
            return db.Translations;
        }

        public override Translation FindByKey(TranslationDto dto)
        {
            return db.Translations.Where(x => x.Name == dto.Name).FirstOrDefault();
        }

        public override void MapFromDtoToEntity(Translation entity, TranslationDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Name = dto.Name;
            entity.En = dto.En;
            entity.Ru = dto.Ru;

        }

        public override TranslationDto MapFromEntityToDto(Translation entity)
        {
            return new TranslationDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                En = entity.En,
                Ru = entity.Ru,
            };
        }
    }
}
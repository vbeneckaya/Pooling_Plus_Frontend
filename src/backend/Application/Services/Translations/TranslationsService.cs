using System;
using System.Collections.Generic;
using System.Linq;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.Translations;
using Domain.Services.UserProvider;

namespace Application.Services.Translations
{
    public class TranslationsService : DictonaryServiceBase<Translation, TranslationDto>, ITranslationsService
    {
        public TranslationsService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public IEnumerable<TranslationDto> GetAll()
        {
            return _dataService.GetDbSet<Translation>().ToList().Select(x=>
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

        public Translation FindByKey(string name)
        {
            return _dataService.GetDbSet<Translation>().Where(x => x.Name == name).FirstOrDefault();
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
using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.LegalPersons;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.BodyTypes
{
    public class LegalPersonService : DictonaryServiceBase<LegalPerson, LegalPersonDto>, ILegalPersonsService
    {
        public LegalPersonService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                                IValidationService validationService, IFieldDispatcherService fieldDispatcherService, IFieldSetterFactory fieldSetterFactory) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory) 
        { }

        public override DetailedValidationResult MapFromDtoToEntity(LegalPerson entity, LegalPersonDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);

            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return null;
        }

        public override LegalPersonDto MapFromEntityToDto(LegalPerson entity)
        {
            return new LegalPersonDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                IsActive = entity.IsActive
            };
        }
        protected override DetailedValidationResult ValidateDto(LegalPersonDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<LegalPerson>()
                                .Where(x => x.Name == dto.Name && x.Id.ToString() != dto.Id)
                                .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "LegalPerson.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<LegalPerson>()
                .Where(i => i.IsActive)
                .OrderBy(x => x.Name)
                .ToList();

            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Name,
                    Value = entity.Id.ToString(),
                };
            }
        }

        protected override IQueryable<LegalPerson> ApplySort(IQueryable<LegalPerson> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }

        public override LegalPerson FindByKey(LegalPersonDto dto)
        {
            return _dataService.GetDbSet<LegalPerson>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }
    }
}

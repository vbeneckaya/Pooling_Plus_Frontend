using Application.Services.Triggers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.PickingTypes;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.PickingTypes
{
    public class PickingTypesService : DictonaryServiceBase<PickingType, PickingTypeDto>, IPickingTypesService
    {
        public PickingTypesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                                   IValidationService validationService, IFieldDispatcherService fieldDispatcherService) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService) 
        { }

        public override DetailedValidationResult MapFromDtoToEntity(PickingType entity, PickingTypeDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            
            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return null;
        }

        protected override DetailedValidationResult ValidateDto(PickingTypeDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<PickingType>()
                                            .Where(x => x.Name == dto.Name && x.Id.ToString() != dto.Id)
                                            .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "PickingType.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override PickingTypeDto MapFromEntityToDto(PickingType entity)
        {
            return new PickingTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                IsActive = entity.IsActive
            };
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var pickingTypes = _dataService.GetDbSet<PickingType>()
                .Where(i => i.IsActive)
                .OrderBy(c => c.Name).ToList();

            foreach (PickingType pickingType in pickingTypes)
            {
                yield return new LookUpDto
                {
                    Name = pickingType.Name,
                    Value = pickingType.Id.ToString(),
                };
            }
        }

        protected override IQueryable<PickingType> ApplySort(IQueryable<PickingType> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }
        public override PickingType FindByKey(PickingTypeDto dto)
        {
            return _dataService.GetDbSet<PickingType>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }
    }
}

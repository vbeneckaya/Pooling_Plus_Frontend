using Application.Shared;
using DAL.Services;
using Domain.Persistables;
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
        public PickingTypesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public override DetailedValidationResult MapFromDtoToEntity(PickingType entity, PickingTypeDto dto)
        {
            var validateResult = ValidateDto(dto);
            if (validateResult.IsError)
            {
                return validateResult;
            }

            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            
            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return new DetailedValidationResult(null, entity.Id.ToString());
        }

        private DetailedValidationResult ValidateDto(PickingTypeDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = new DetailedValidationResult();

            if (string.IsNullOrEmpty(dto.Name))
            {
                result.AddError(nameof(dto.Name), "pickingType.emptyName".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            var hasDuplicates = _dataService.GetDbSet<PickingType>()
                                            .Where(x => x.Name == dto.Name && x.Id.ToString() != dto.Id)
                                            .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "pickingType.duplicated".Translate(lang), ValidationErrorType.DuplicatedRecord);
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

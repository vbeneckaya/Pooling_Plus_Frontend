using Application.Shared;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.BodyTypes;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Services.BodyTypes
{
    public class BodyTypesService : DictonaryServiceBase<BodyType, BodyTypeDto>, IBodyTypesService
    {
        public BodyTypesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public override ValidateResult MapFromDtoToEntity(BodyType entity, BodyTypeDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);

            var validateResult = ValidateDto(dto);
            if (validateResult.IsError)
            {
                return validateResult;
            }

            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return new ValidateResult(null, entity.Id.ToString());
        }

        public override BodyTypeDto MapFromEntityToDto(BodyType entity)
        {
            return new BodyTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                IsActive = entity.IsActive
            };
        }
        private ValidateResult ValidateDto(BodyTypeDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidattionResult result = new DetailedValidattionResult();

            if (string.IsNullOrEmpty(dto.Name))
            {
                result.AddError(nameof(dto.Name), "bodyType.emptyName".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            var existingRecord = this.FindByKey(dto);
            var hasDuplicates = existingRecord != null && existingRecord.Id != dto.Id.ToGuid();

            if (hasDuplicates)
            {
                result.AddError("duplicatedBodyType", "bodyType.duplicated".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<BodyType>()
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

        protected override IQueryable<BodyType> ApplySort(IQueryable<BodyType> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }

        public override BodyType FindByKey(BodyTypeDto dto)
        {
            return _dataService.GetDbSet<BodyType>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }
    }
}

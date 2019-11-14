using Application.Shared;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Tonnages;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Services.Tonnages
{
    public class TonnagesService : DictonaryServiceBase<Tonnage, TonnageDto>, ITonnagesService
    {
        public TonnagesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public override ValidateResult MapFromDtoToEntity(Tonnage entity, TonnageDto dto)
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

        public override TonnageDto MapFromEntityToDto(Tonnage entity)
        {
            return new TonnageDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                IsActive = entity.IsActive
            };
        }

        private ValidateResult ValidateDto(TonnageDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidattionResult result = new DetailedValidattionResult();

            if (string.IsNullOrEmpty(dto.Name))
            {
                result.AddError(nameof(dto.Name), "tonnage.emptyName".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            var existingRecord = this.FindByKey(dto);
            var hasDuplicates = existingRecord != null && existingRecord.Id != dto.Id.ToGuid();

            if (hasDuplicates)
            {
                result.AddError("duplicatedTonnage", "tonnage.duplicated".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<Tonnage>()
                .Where(i => i.IsActive)
                .OrderBy(x => x.Name)
                .ToList();

            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Name,
                    Value = entity.Id.ToString()
                };
            }
        }

        protected override IQueryable<Tonnage> ApplySort(IQueryable<Tonnage> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }

        public override Tonnage FindByKey(TonnageDto dto)
        {
            return _dataService.GetDbSet<Tonnage>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }
    }
}

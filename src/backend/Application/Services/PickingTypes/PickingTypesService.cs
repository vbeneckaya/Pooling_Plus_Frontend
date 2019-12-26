using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
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
    public class PickingTypesService : DictionaryServiceBase<PickingType, PickingTypeDto>, IPickingTypesService
    {
        public PickingTypesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                                   IValidationService validationService, IFieldDispatcherService fieldDispatcherService, 
                                   IFieldSetterFactory fieldSetterFactory, IAppConfigurationService configurationService) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, configurationService
                  ) 
        { }

        public override DetailedValidationResult MapFromDtoToEntity(PickingType entity, PickingTypeDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            
            entity.Name = dto.Name;
            entity.CompanyId = dto.CompanyId?.Value?.ToGuid();
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
            var user = _userProvider.GetCurrentUser();

            return new PickingTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                IsActive = entity.IsActive,
                CompanyId = entity.CompanyId == null ? null : new LookUpDto(entity.CompanyId.ToString()),
                IsEditable = user.CompanyId == null || entity.CompanyId != null
            };
        }

        protected override IEnumerable<PickingTypeDto> FillLookupNames(IEnumerable<PickingTypeDto> dtos)
        {
            var companyIds = dtos.Where(x => !string.IsNullOrEmpty(x.CompanyId?.Value))
                         .Select(x => x.CompanyId.Value.ToGuid())
                         .ToList();

            var companies = _dataService.GetDbSet<Company>()
                                           .Where(x => companyIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.CompanyId?.Value)
                    && companies.TryGetValue(dto.CompanyId.Value, out Company company))
                {
                    dto.CompanyId.Name = company.Name;
                }
                yield return dto;
            }
        }

        protected override ExcelMapper<PickingTypeDto> CreateExcelMapper()
        {
            return base.CreateExcelMapper()
                .MapColumn(w => w.CompanyId, new DictionaryReferenceExcelColumn(GetCompanyIdByName));
        }
        private Guid? GetCompanyIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Company>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            var pickingTypes = _dataService.GetDbSet<PickingType>()
                .Where(i => i.IsActive)
                .OrderBy(c => c.Name).ToList();

            var empty = new LookUpDto
            {
                Name = "emptyValue".Translate(lang),
                Value = LookUpDto.EmptyValue,
                IsFilterOnly = true
            };
            yield return empty;

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

        public override IQueryable<PickingType> ApplyRestrictions(IQueryable<PickingType> query)
        {
            var currentUserId = _userProvider.GetCurrentUserId();
            var user = _dataService.GetById<User>(currentUserId.Value);

            // Local user restrictions

            if (user?.CompanyId != null)
            {
                query = query.Where(i => i.CompanyId == user.CompanyId || i.CompanyId == null);
            }

            return query;
        }

        public override PickingType FindByKey(PickingTypeDto dto)
        {
            return _dataService.GetDbSet<PickingType>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }

        public override UserConfigurationDictionaryItem GetDictionaryConfiguration(Guid id)
        {
            var user = _userProvider.GetCurrentUser();
            var configuration = base.GetDictionaryConfiguration(id);

            var companyId = configuration.Columns.First(i => i.Name.ToLower() == nameof(PickingType.CompanyId).ToLower());
            companyId.IsReadOnly = user.CompanyId != null;

            return configuration;
        }
    }
}

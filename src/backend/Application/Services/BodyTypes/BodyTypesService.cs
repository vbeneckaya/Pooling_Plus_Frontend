using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Services.BodyTypes;
using Domain.Services.FieldProperties;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.BodyTypes
{
    public class BodyTypesService : DictionaryServiceBase<BodyType, BodyTypeDto>, IBodyTypesService
    {
        public BodyTypesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                                IValidationService validationService, IFieldDispatcherService fieldDispatcherService, 
                                IFieldSetterFactory fieldSetterFactory, IAppConfigurationService configurationService) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, configurationService) 
        { }

        public override DetailedValidationResult MapFromDtoToEntity(BodyType entity, BodyTypeDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);

            entity.Name = dto.Name;
            entity.CompanyId = dto.CompanyId?.Value?.ToGuid();
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return null;
        }

        public override BodyTypeDto MapFromEntityToDto(BodyType entity)
        {
            return new BodyTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                CompanyId = entity.CompanyId == null ? null : new LookUpDto(entity.CompanyId.ToString()),
                IsActive = entity.IsActive
            };
        }
        protected override DetailedValidationResult ValidateDto(BodyTypeDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<BodyType>()
                                .Where(x => x.Name == dto.Name && x.Id.ToString() != dto.Id)
                                .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "BodyType.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        protected override IEnumerable<BodyTypeDto> FillLookupNames(IEnumerable<BodyTypeDto> dtos)
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

        public override IQueryable<BodyType> ApplyRestrictions(IQueryable<BodyType> query)
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

        public override UserConfigurationDictionaryItem GetDictionaryConfiguration(Guid id)
        {
            var entity = _dataService.GetById<BodyType>(id);

            var configuration = base.GetDictionaryConfiguration(id);

            var companyId = configuration.Columns.First(i => i.Name.ToLower() == nameof(BodyType.CompanyId).ToLower());
            companyId.IsReadOnly = entity.CompanyId != null;

            return configuration;
        }
    }
}

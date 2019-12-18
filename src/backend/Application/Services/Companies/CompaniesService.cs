using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.Companies;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Services.AppConfiguration;

namespace Application.Services.BodyTypes
{
    public class CompaniesService : DictionaryServiceBase<Company, CompanyDto>, ICompaniesService
    {
        public CompaniesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                                IValidationService validationService, IFieldDispatcherService fieldDispatcherService, 
                                IFieldSetterFactory fieldSetterFactory, IAppConfigurationService configurationService) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, configurationService) 
        { }

        public override DetailedValidationResult MapFromDtoToEntity(Company entity, CompanyDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);

            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return null;
        }

        public override CompanyDto MapFromEntityToDto(Company entity)
        {
            return new CompanyDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                IsActive = entity.IsActive
            };
        }
        protected override DetailedValidationResult ValidateDto(CompanyDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<Company>()
                                .Where(x => x.Name == dto.Name && x.Id.ToString() != dto.Id)
                                .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "Company.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<Company>()
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

        public override IQueryable<Company> ApplyRestrictions(IQueryable<Company> query)
        {
            return query;
        }

        protected override IQueryable<Company> ApplySort(IQueryable<Company> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }

        public override Company FindByKey(CompanyDto dto)
        {
            return _dataService.GetDbSet<Company>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }
    }
}

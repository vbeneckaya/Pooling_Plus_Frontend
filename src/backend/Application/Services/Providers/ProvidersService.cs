using System;
using System.Collections.Generic;
using System.Linq;
using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Services.FieldProperties;
using Domain.Services.Providers;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;

namespace Application.Services.Providers
{
    public class ProvidersService : DictionaryServiceBase<Provider, ProviderDto>, IProvidersService
    {
        public ProvidersService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                                IValidationService validationService, IFieldDispatcherService fieldDispatcherService, 
                                IFieldSetterFactory fieldSetterFactory, IAppConfigurationService configurationService) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, configurationService) 
        { }

        public override DetailedValidationResult MapFromDtoToEntity(Provider entity, ProviderDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);

            entity.Name = dto.Name;
            entity.Inn = dto.Inn;
            entity.Cpp = dto.Cpp;
            entity.LegalAddress = dto.LegalAddress;
            entity.ActualAddress = dto.ActualAddress;
            entity.ContactPerson = dto.ContactPerson;
            entity.ContactPhone = dto.ContactPhone;
            entity.Email = dto.Email;
            entity.ReportId = dto.ReportId;
            entity.ReportPageNameForMobile = dto.ReportPageNameForMobile;
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return null;
        }

        public override ProviderDto MapFromEntityToDto(Provider entity)
        {
            return new ProviderDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                Cpp = entity.Cpp,
                Inn = entity.Inn,
                ActualAddress = entity.ActualAddress,
                LegalAddress = entity.LegalAddress,
                ContactPerson = entity.ContactPerson,
                ContactPhone = entity.ContactPhone,
                Email = entity.Email,
                ReportId = entity.ReportId,
                ReportPageNameForMobile = entity.ReportPageNameForMobile,
                IsActive = entity.IsActive
            };
        }
        protected override DetailedValidationResult ValidateDto(ProviderDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<Provider>()
                                .Where(x => x.Name == dto.Name && x.Id.ToString() != dto.Id)
                                .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "Provider.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<Provider>()
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

        protected override IQueryable<Provider> ApplySort(IQueryable<Provider> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }

        public override Provider FindByKey(ProviderDto dto)
        {
            return _dataService.GetDbSet<Provider>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }
    }
}

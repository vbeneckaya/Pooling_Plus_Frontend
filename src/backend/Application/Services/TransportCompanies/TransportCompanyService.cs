using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.Translations;
using Domain.Services.TransportCompanies;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.TransportCompanies
{
    public class TransportCompaniesService : DictionaryServiceBase<TransportCompany, TransportCompanyDto>, ITransportCompaniesService
    {
        public TransportCompaniesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                                         IValidationService validationService, IFieldDispatcherService fieldDispatcherService, IFieldSetterFactory fieldSetterFactory) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory) 
        { }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            var carriers = _dataService.GetDbSet<TransportCompany>()
                .Where(i => i.IsActive)
                .OrderBy(c => c.Title)
                .ToList();

            var empty = new LookUpDto
            {
                Name = "emptyValue".Translate(lang),
                Value = LookUpDto.EmptyValue,
                IsFilterOnly = true
            };
            yield return empty;

            foreach (TransportCompany carrier in carriers)
            {
                yield return new LookUpDto
                {
                    Name = carrier.Title,
                    Value = carrier.Id.ToString()
                };
            }
        }

        public override DetailedValidationResult MapFromDtoToEntity(TransportCompany entity, TransportCompanyDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Title = dto.Title;
            entity.CompanyId = dto.CompanyId?.Value?.ToGuid();
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return null;
        }

        protected override DetailedValidationResult ValidateDto(TransportCompanyDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var companyId = dto.CompanyId?.Value?.ToGuid();

            var hasDuplicates = _dataService.GetDbSet<TransportCompany>()
                                            .Where(x => !string.IsNullOrEmpty(dto.Title) && x.Title.ToLower() == dto.Title.ToLower())
                                            .Where(x => x.Id.ToString() != dto.Id)
                                            .Where(x =>  x.CompanyId == null || x.CompanyId == companyId)
                                            .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Title), "TransportCompany.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override TransportCompanyDto MapFromEntityToDto(TransportCompany entity)
        {
            return new TransportCompanyDto
            {
                Id = entity.Id.ToString(),
                Title = entity.Title,
                CompanyId = entity.CompanyId == null ? null : new LookUpDto(entity.CompanyId.ToString()),
                IsActive = entity.IsActive
            };
        }

        protected override IEnumerable<TransportCompanyDto> FillLookupNames(IEnumerable<TransportCompanyDto> dtos)
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

        protected override IQueryable<TransportCompany> ApplySort(IQueryable<TransportCompany> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Title)
                .ThenBy(i => i.Id);
        }

        public override TransportCompany FindByKey(TransportCompanyDto dto)
        {
            return _dataService.GetDbSet<TransportCompany>()
                .FirstOrDefault(i => i.Title == dto.Title);
        }

        protected override ExcelMapper<TransportCompanyDto> CreateExcelMapper()
        {
            return base.CreateExcelMapper()
                .MapColumn(w => w.CompanyId, new DictionaryReferenceExcelColumn(GetCompanyIdByName));
        }

        private Guid? GetCompanyIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Company>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }
    }
}
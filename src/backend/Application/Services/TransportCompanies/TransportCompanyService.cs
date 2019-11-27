using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.Translations;
using Domain.Services.TransportCompanies;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.TransportCompanies
{
    public class TransportCompaniesService : DictonaryServiceBase<TransportCompany, TransportCompanyDto>, ITransportCompaniesService
    {
        public TransportCompaniesService(ICommonDataService dataService, IUserProvider userProvider, IServiceProvider serviceProvider) 
            : base(dataService, userProvider, serviceProvider) 
        { }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var carriers = _dataService.GetDbSet<TransportCompany>()
                .Where(i => i.IsActive)
                .OrderBy(c => c.Title)
                .ToList();

            foreach (TransportCompany carrier in carriers)
            {
                yield return new LookUpDto
                {
                    Name = carrier.Title,
                    Value = carrier.Id.ToString()
                };
            }
        }

        public override ValidateResult MapFromDtoToEntity(TransportCompany entity, TransportCompanyDto dto)
        {
            var validateResult = ValidateDto(dto);
            if (validateResult.IsError)
            {
                return validateResult;
            }

            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Title = dto.Title;
            entity.ContractNumber = dto.ContractNumber;
            entity.DateOfPowerOfAttorney = dto.DateOfPowerOfAttorney;
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return new ValidateResult(null, entity.Id.ToString());
        }

        private ValidateResult ValidateDto(TransportCompanyDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidattionResult result = new DetailedValidattionResult();

            if (string.IsNullOrEmpty(dto.Title))
            {
                result.AddError(nameof(dto.Title), "transportCompany.emptyTitle".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            var hasDuplicates = _dataService.GetDbSet<TransportCompany>()
                                            .Where(x => !string.IsNullOrEmpty(dto.Title) && x.Title.ToLower() == dto.Title.ToLower() && x.Id.ToString() != dto.Id)
                                            .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Title), "transportCompany.duplicated".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override TransportCompanyDto MapFromEntityToDto(TransportCompany entity)
        {
            return new TransportCompanyDto
            {
                Id = entity.Id.ToString(),
                Title = entity.Title,
                ContractNumber = entity.ContractNumber,
                DateOfPowerOfAttorney = entity.DateOfPowerOfAttorney,
                IsActive = entity.IsActive
            };
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
    }
}
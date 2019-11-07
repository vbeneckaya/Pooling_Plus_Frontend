using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.TransportCompanies;
using Domain.Services.UserProvider;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.TransportCompanies
{
    public class TransportCompaniesService : DictonaryServiceBase<TransportCompany, TransportCompanyDto>, ITransportCompaniesService
    {
        public TransportCompaniesService(ICommonDataService dataService, IUserProvider userProvider, ILogger<TransportCompaniesService> logger) : base(dataService, userProvider, logger) { }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var carriers = _dataService.GetDbSet<TransportCompany>().OrderBy(c => c.Title).ToList();
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
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Title = dto.Title;
            entity.ContractNumber = dto.ContractNumber;
            entity.DateOfPowerOfAttorney = dto.DateOfPowerOfAttorney;

            return new ValidateResult(entity.Id.ToString());
        }

        public override TransportCompanyDto MapFromEntityToDto(TransportCompany entity)
        {
            return new TransportCompanyDto
            {
                Id = entity.Id.ToString(),
                Title = entity.Title,
                ContractNumber = entity.ContractNumber,
                DateOfPowerOfAttorney = entity.DateOfPowerOfAttorney,
                /*end of map entity to dto fields*/
            };
        }

        protected override IQueryable<TransportCompany> ApplySort(IQueryable<TransportCompany> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Title)
                .ThenBy(i => i.Id);
        }
    }
}
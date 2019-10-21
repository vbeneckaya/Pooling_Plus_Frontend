using System;
using Application.Shared;
using Domain.Persistables;
using Domain.Services.TransportCompanies;
using System.Linq;
using Domain.Shared;
using System.Collections.Generic;
using DAL.Services;
using Domain.Services.UserProvider;

namespace Application.Services.TransportCompanies
{
    public class TransportCompaniesService : DictonaryServiceBase<TransportCompany, TransportCompanyDto>, ITransportCompaniesService
    {
        public TransportCompaniesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public override TransportCompany FindByKey(TransportCompanyDto dto)
        {
            return _dataService.GetDbSet<TransportCompany>().Where(x => x.Title == dto.Title).FirstOrDefault();
        }

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

        public override void MapFromDtoToEntity(TransportCompany entity, TransportCompanyDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Title = dto.Title;
            entity.ContractNumber = dto.ContractNumber;
            entity.DateOfPowerOfAttorney = dto.DateOfPowerOfAttorney;
            /*end of map dto to entity fields*/
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
    }
}
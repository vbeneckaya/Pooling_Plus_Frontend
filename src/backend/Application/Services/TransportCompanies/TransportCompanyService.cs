using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.TransportCompanies;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.TransportCompanies
{
    public class TransportCompaniesService : DictonaryServiceBase<TransportCompany, TransportCompanyDto>, ITransportCompaniesService
    {
        public TransportCompaniesService(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override DbSet<TransportCompany> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.TransportCompanies;
        }

        public override void MapFromDtoToEntity(TransportCompany entity, TransportCompanyDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            
            entity.Title = dto.Title;
            entity.ContractNumber = dto.ContractNumber;
            entity.DateOfPowerOfAttorney = dto.DateOfPowerOfAttorney;
            /*end of fields*/
        }

        public override TransportCompanyDto MapFromEntityToDto(TransportCompany entity)
        {
            return new TransportCompanyDto
            {
                Id = entity.Id.ToString(),
                Title = entity.Title,
                ContractNumber = entity.ContractNumber,
                DateOfPowerOfAttorney = entity.DateOfPowerOfAttorney,
                /*end of fields*/
            };
        }
    }
}
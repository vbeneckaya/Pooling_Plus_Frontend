using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.TransportСompanies;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.TransportСompanies
{
    public class TransportСompaniesService : DictonaryServiceBase<TransportСompany, TransportСompanyDto>, ITransportСompaniesService
    {
        public TransportСompaniesService(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override DbSet<TransportСompany> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.TransportСompanies;
        }

        public override void MapFromDtoToEntity(TransportСompany entity, TransportСompanyDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            /*end of fields*/
        }

        public override TransportСompanyDto MapFromEntityToDto(TransportСompany entity)
        {
            return new TransportСompanyDto
            {
                Id = entity.Id.ToString(),
                /*end of fields*/
            };
        }
    }
}
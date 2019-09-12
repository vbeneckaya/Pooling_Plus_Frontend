using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Transportations;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Transportations
{
    public class TransportationsService : GridServiceBase<Transportation, TransportationDto>, ITransportationsService
    {
        public TransportationsService(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override DbSet<Transportation> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Transportations;
        }

        public override void MapFromDtoToEntity(Transportation entity, TransportationDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            /*end of fields*/
        }

        public override TransportationDto MapFromEntityToDto(Transportation entity)
        {
            return new TransportationDto
            {
                Id = entity.Id.ToString(),
                /*end of fields*/
            };
        }
    }
}
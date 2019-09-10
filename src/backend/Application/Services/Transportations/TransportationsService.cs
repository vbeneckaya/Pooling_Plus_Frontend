using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Transportations;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Transportations
{
    public class TransportationsService :  DictonaryServiceBase<Transportation, TransportationDto>, ITransportationsService
    {
        public TransportationsService(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override DbSet<Transportation> UseDbSet(AppDbContext dbContext)
        {
            return db.Transportations;
        }

        public override void MapFromDtoToEntity(Transportation entity, TransportationDto dto)
        {
        }

        public override TransportationDto MapFromEntityToDto(Transportation entity)
        {
            return new TransportationDto
            {
                Id = entity.Id.ToString()
            };
        }
    }
}
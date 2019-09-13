using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Tariffs;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Tariffs
{
    public class TariffsService : DictonaryServiceBase<Tariff, TariffDto>, ITariffsService
    {
        public TariffsService(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override DbSet<Tariff> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Tariffs;
        }

        public override void MapFromDtoToEntity(Tariff entity, TariffDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            /*end of fields*/
        }

        public override TariffDto MapFromEntityToDto(Tariff entity)
        {
            return new TariffDto
            {
                Id = entity.Id.ToString(),
                /*end of fields*/
            };
        }
    }
}
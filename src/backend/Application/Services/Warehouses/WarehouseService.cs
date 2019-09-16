using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Warehouses;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Warehouses
{
    public class WarehousesService : DictonaryServiceBase<Warehouse, WarehouseDto>, IWarehousesService
    {
        public WarehousesService(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override DbSet<Warehouse> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Warehouses;
        }

        public override void MapFromDtoToEntity(Warehouse entity, WarehouseDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            /*end of fields*/
        }

        public override WarehouseDto MapFromEntityToDto(Warehouse entity)
        {
            return new WarehouseDto
            {
                Id = entity.Id.ToString(),
                /*end of fields*/
            };
        }
    }
}
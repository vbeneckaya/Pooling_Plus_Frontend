using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Extensions;
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
            entity.TheNameOfTheWarehouse = dto.TheNameOfTheWarehouse;
            entity.SoldToNumber = dto.SoldToNumber;
            entity.Region = dto.Region;
            entity.City = dto.City;
            entity.Address = dto.Address;
            entity.TypeOfEquipment = dto.TypeOfEquipment;
            entity.LeadtimeDays = dto.LeadtimeDays;
            entity.CustomerWarehouse = dto.CustomerWarehouse;
            entity.UseTypeOfEquipment = dto.UseTypeOfEquipment;
            /*end of map dto to entity fields*/
        }

        public override WarehouseDto MapFromEntityToDto(Warehouse entity)
        {
            return new WarehouseDto
            {
                Id = entity.Id.ToString(),
                TheNameOfTheWarehouse = entity.TheNameOfTheWarehouse,
                SoldToNumber = entity.SoldToNumber,
                Region = entity.Region,
                City = entity.City,
                Address = entity.Address,
                TypeOfEquipment = entity.TypeOfEquipment,
                LeadtimeDays = entity.LeadtimeDays,
                CustomerWarehouse = entity.CustomerWarehouse,
                UseTypeOfEquipment = entity.UseTypeOfEquipment,
                /*end of map entity to dto fields*/
            };
        }
    }
}
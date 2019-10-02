using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Extensions;
using Domain.Services.Warehouses;
using Microsoft.EntityFrameworkCore;
using Domain.Shared;
using System.Collections.Generic;
using System.Linq;

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

        public override Warehouse FindByKey(WarehouseDto dto)
        {
            return db.Warehouses.Where(x => x.SoldToNumber == dto.SoldToNumber).FirstOrDefault();
        }

        public override void MapFromDtoToEntity(Warehouse entity, WarehouseDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.WarehouseName = dto.WarehouseName;
            entity.SoldToNumber = dto.SoldToNumber;
            entity.Region = dto.Region;
            entity.City = dto.City;
            entity.Address = dto.Address;
            entity.PickingType = dto.PickingType;
            entity.LeadtimeDays = dto.LeadtimeDays;
            entity.CustomerWarehouse = dto.CustomerWarehouse;
            entity.UsePickingType = dto.UsePickingType;
            /*end of map dto to entity fields*/
        }

        public override WarehouseDto MapFromEntityToDto(Warehouse entity)
        {
            return new WarehouseDto
            {
                Id = entity.Id.ToString(),
                WarehouseName = entity.WarehouseName,
                SoldToNumber = entity.SoldToNumber,
                Region = entity.Region,
                City = entity.City,
                Address = entity.Address,
                PickingType = entity.PickingType,
                LeadtimeDays = entity.LeadtimeDays,
                CustomerWarehouse = entity.CustomerWarehouse,
                UsePickingType = entity.UsePickingType,
                /*end of map entity to dto fields*/
            };
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            foreach (Warehouse wh in db.Warehouses.OrderBy(w => w.WarehouseName))
            {
                yield return new LookUpDto
                {
                    Name = wh.WarehouseName,
                    Value = wh.WarehouseName//wh.Id.ToString()
                };
            }
        }
    }
}
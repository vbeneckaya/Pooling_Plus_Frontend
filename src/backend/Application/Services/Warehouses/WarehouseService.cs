using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Warehouses;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using DAL.Queries;
using System.Collections.Generic;
using Domain.Shared;

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

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = db.Warehouses.OrderBy(x => x.WarehouseName).ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.WarehouseName,
                    Value = entity.Id.ToString()
                };
            }
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
            entity.PickingTypeId = string.IsNullOrEmpty(dto.PickingTypeId) ? (Guid?)null : Guid.Parse(dto.PickingTypeId);
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
                PickingTypeId = entity.PickingTypeId?.ToString(),
                LeadtimeDays = entity.LeadtimeDays,
                CustomerWarehouse = entity.CustomerWarehouse,
                UsePickingType = entity.UsePickingType,
                /*end of map entity to dto fields*/
            };
        }

        protected override ExcelMapper<WarehouseDto> CreateExcelMapper()
        {
            return new ExcelMapper<WarehouseDto>(db)
                .MapColumn(w => w.PickingTypeId, new DictionaryReferenceExcelColumn(GetPickingTypeIdByName, GetPickingTypeNameById));
        }

        private Guid? GetPickingTypeIdByName(string name)
        {
            var entry = db.PickingTypes.Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        private string GetPickingTypeNameById(Guid id)
        {
            var entry = db.PickingTypes.GetById(id);
            return entry?.Name;
        }
    }
}
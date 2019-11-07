using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using DAL.Queries;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.UserProvider;
using Domain.Services.Warehouses;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Warehouses
{
    public class WarehousesService : DictonaryServiceBase<Warehouse, WarehouseDto>, IWarehousesService
    {
        public WarehousesService(ICommonDataService dataService, IUserProvider userProvider, ILogger<WarehousesService> logger) : base(dataService, userProvider, logger) { }

        public WarehouseDto GetBySoldTo(string soldToNumber)
        {
            var entity = _dataService.GetDbSet<Warehouse>().Where(x => x.SoldToNumber == soldToNumber).FirstOrDefault();
            return MapFromEntityToDto(entity);
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<Warehouse>().OrderBy(x => x.WarehouseName).ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.WarehouseName,
                    Value = entity.Id.ToString()
                };
            }
        }

        public override ValidateResult MapFromDtoToEntity(Warehouse entity, WarehouseDto dto)
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

            return new ValidateResult(entity.Id.ToString());
        }

        public override WarehouseDto MapFromEntityToDto(Warehouse entity)
        {
            if (entity == null)
            {
                return null;
            }
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
                /*end of map entity to dto fields*/
            };
        }

        protected override ExcelMapper<WarehouseDto> CreateExcelMapper()
        {
            return new ExcelMapper<WarehouseDto>(_dataService, _userProvider)
                .MapColumn(w => w.PickingTypeId, new DictionaryReferenceExcelColumn(GetPickingTypeIdByName, GetPickingTypeNameById));
        }

        private Guid? GetPickingTypeIdByName(string name)
        {
            var entry = _dataService.GetDbSet<PickingType>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        private string GetPickingTypeNameById(Guid id)
        {
            var entry = _dataService.GetDbSet<PickingType>().GetById(id);
            return entry?.Name;
        }

        protected override IQueryable<Warehouse> ApplySort(IQueryable<Warehouse> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.WarehouseName)
                .ThenBy(i => i.Id);
        }
    }
}
using System;
using Application.Shared;
using Domain.Persistables;
using Domain.Services.Warehouses;
using System.Linq;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using DAL.Queries;
using System.Collections.Generic;
using Domain.Shared;
using DAL.Services;
using Domain.Services.UserProvider;

namespace Application.Services.Warehouses
{
    public class WarehousesService : DictonaryServiceBase<Warehouse, WarehouseDto>, IWarehousesService
    {
        public WarehousesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

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

        public override Warehouse FindByKey(WarehouseDto dto)
        {
            return _dataService.GetDbSet<Warehouse>().Where(x => x.SoldToNumber == dto.SoldToNumber).FirstOrDefault();
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
            /*end of map dto to entity fields*/
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
    }
}
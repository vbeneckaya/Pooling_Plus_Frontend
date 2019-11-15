using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using AutoMapper;
using DAL.Queries;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Services.UserProvider;
using Domain.Services.Warehouses;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Warehouses
{
    public class WarehousesService : DictonaryServiceBase<Warehouse, WarehouseDto>, IWarehousesService
    {
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;

        public WarehousesService(ICommonDataService dataService, IUserProvider userProvider, IHistoryService historyService) 
            : base(dataService, userProvider)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
        }

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
            var setter = new FieldSetter<Warehouse>(entity, _historyService);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id), ignoreChanges: true);
            setter.UpdateField(e => e.WarehouseName, dto.WarehouseName);
            setter.UpdateField(e => e.SoldToNumber, dto.SoldToNumber);
            setter.UpdateField(e => e.Region, dto.Region);
            setter.UpdateField(e => e.City, dto.City);
            setter.UpdateField(e => e.Address, dto.Address);
            setter.UpdateField(e => e.PickingTypeId, string.IsNullOrEmpty(dto.PickingTypeId) ? (Guid?)null : Guid.Parse(dto.PickingTypeId), nameLoader: GetPickingTypeNameById);
            setter.UpdateField(e => e.LeadtimeDays, dto.LeadtimeDays);
            setter.UpdateField(e => e.CustomerWarehouse, dto.CustomerWarehouse);
            setter.UpdateField(e => e.PickingFeatures, dto.PickingFeatures);

            setter.ApplyAfterActions();
            setter.SaveHistoryLog();

            string errors = setter.ValidationErrors;
            return new ValidateResult(errors, entity.Id.ToString());
        }

        public override WarehouseDto MapFromEntityToDto(Warehouse entity)
        {
            if (entity == null)
            {
                return null;
            }
            return _mapper.Map<WarehouseDto>(entity);
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
            return GetPickingTypeNameById((Guid?)id);
        }

        private string GetPickingTypeNameById(Guid? id)
        {
            var entry = _dataService.GetDbSet<PickingType>().FirstOrDefault(x => x.Id == id);
            return entry?.Name;
        }

        protected override IQueryable<Warehouse> ApplySort(IQueryable<Warehouse> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.WarehouseName)
                .ThenBy(i => i.Id);
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Warehouse, WarehouseDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.PickingTypeId, e => e.MapFrom((s, t) => s.PickingTypeId?.ToString()));
            });
            return result;
        }
    }
}
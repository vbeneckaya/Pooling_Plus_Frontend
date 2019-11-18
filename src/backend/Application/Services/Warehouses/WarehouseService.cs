using Application.BusinessModels.Warehouses.Handlers;
using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using AutoMapper;
using DAL.Queries;
using DAL.Services;
using Domain.Enums;
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
            bool isNew = string.IsNullOrEmpty(dto.Id);
            var setter = new FieldSetter<Warehouse>(entity, _historyService);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id), ignoreChanges: true);
            setter.UpdateField(e => e.WarehouseName, dto.WarehouseName, new WarehouseNameHandler(_dataService, _historyService));
            setter.UpdateField(e => e.SoldToNumber, dto.SoldToNumber);
            setter.UpdateField(e => e.Region, dto.Region, new RegionHandler(_dataService, _historyService));
            setter.UpdateField(e => e.City, dto.City, new CityHandler(_dataService, _historyService));
            setter.UpdateField(e => e.Address, dto.Address, new AddressHandler(_dataService, _historyService));
            setter.UpdateField(e => e.PickingTypeId, string.IsNullOrEmpty(dto.PickingTypeId) ? (Guid?)null : Guid.Parse(dto.PickingTypeId), 
                               new PickingTypeIdHandler(_dataService, _historyService), nameLoader: GetPickingTypeNameById);
            setter.UpdateField(e => e.LeadtimeDays, dto.LeadtimeDays, new LeadtimeDaysHandler(_dataService, _historyService));
            setter.UpdateField(e => e.CustomerWarehouse, dto.CustomerWarehouse);
            setter.UpdateField(e => e.PickingFeatures, dto.PickingFeatures);

            setter.ApplyAfterActions();
            setter.SaveHistoryLog();

            if (isNew)
            {
                var validStatuses = new[] { OrderState.Draft, OrderState.Created, OrderState.InShipping };
                var orders = _dataService.GetDbSet<Order>()
                                         .Where(x => x.SoldTo == entity.SoldToNumber
                                                    && x.DeliveryWarehouseId == null
                                                    && validStatuses.Contains(x.Status)
                                                    && (x.ShippingId == null || x.OrderShippingStatus == ShippingState.ShippingCreated))
                                         .ToList();
                foreach (var order in orders)
                {
                    order.DeliveryWarehouseId = entity.Id;
                }
            }

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
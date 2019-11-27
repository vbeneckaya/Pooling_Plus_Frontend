using Application.BusinessModels.Warehouses.Handlers;
using Application.Services.Addresses;
using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using AutoMapper;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Services.Translations;
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
        private readonly ICleanAddressService _cleanAddressService;

        public WarehousesService(ICommonDataService dataService, IUserProvider userProvider, 
                                 IHistoryService historyService, ICleanAddressService cleanAddressService) 
            : base(dataService, userProvider)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
            _cleanAddressService = cleanAddressService;
        }

        public WarehouseDto GetBySoldTo(string soldToNumber)
        {
            var entity = _dataService.GetDbSet<Warehouse>().Where(x => x.SoldToNumber == soldToNumber).FirstOrDefault();
            return MapFromEntityToDto(entity);
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<Warehouse>()
                                       .Where(x => x.IsActive)
                                       .OrderBy(x => x.WarehouseName)
                                       .ToList();
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

        public override DetailedValidationResult MapFromDtoToEntity(Warehouse entity, WarehouseDto dto)
        {
            var validateResult = ValidateDto(dto);
            if (validateResult.IsError)
            {
                return validateResult;
            }

            bool isNew = string.IsNullOrEmpty(dto.Id);
            var setter = new FieldSetter<Warehouse>(entity, _historyService);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id), ignoreChanges: true);
            setter.UpdateField(e => e.WarehouseName, dto.WarehouseName, new WarehouseNameHandler(_dataService, _historyService));
            setter.UpdateField(e => e.SoldToNumber, dto.SoldToNumber);
            setter.UpdateField(e => e.Region, dto.Region, new RegionHandler(_dataService, _historyService));
            setter.UpdateField(e => e.City, dto.City, new CityHandler(_dataService, _historyService));
            setter.UpdateField(e => e.Address, dto.Address, new AddressHandler(_dataService, _historyService, _cleanAddressService));
            setter.UpdateField(e => e.PickingTypeId, string.IsNullOrEmpty(dto.PickingTypeId) ? (Guid?)null : Guid.Parse(dto.PickingTypeId), 
                               new PickingTypeIdHandler(_dataService, _historyService), nameLoader: GetPickingTypeNameById);
            setter.UpdateField(e => e.LeadtimeDays, dto.LeadtimeDays, new LeadtimeDaysHandler(_dataService, _historyService));
            setter.UpdateField(e => e.CustomerWarehouse, dto.CustomerWarehouse);
            setter.UpdateField(e => e.PickingFeatures, dto.PickingFeatures, new PickingFeaturesHandler(_dataService, _historyService));
            setter.UpdateField(e => e.DeliveryType, string.IsNullOrEmpty(dto.DeliveryType) ? (DeliveryType?)null : MapFromStateDto<DeliveryType>(dto.DeliveryType), new DeliveryTypeHandler(_dataService, _historyService));
            setter.UpdateField(e => e.IsActive, dto.IsActive ?? true, ignoreChanges: true);

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
            return new DetailedValidationResult(errors, entity.Id.ToString());
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
            string lang = _userProvider.GetCurrentUser()?.Language;
            return new ExcelMapper<WarehouseDto>(_dataService, _userProvider)
                .MapColumn(w => w.PickingTypeId, new DictionaryReferenceExcelColumn(GetPickingTypeIdByName, GetPickingTypeNameById))
                .MapColumn(w => w.DeliveryType, new EnumExcelColumn<DeliveryType>(lang));
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

        protected override IQueryable<Warehouse> ApplySearch(IQueryable<Warehouse> query, SearchFormDto form)
        {
            if (string.IsNullOrEmpty(form.Search)) return query;

            var search = form.Search.ToLower();

            var isInt = int.TryParse(search, out int searchInt);

            var pickingTypes = _dataService.GetDbSet<PickingType>()
                .Where(i => i.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                .Select(i => i.Id).ToList();

            var deliveryTypeNames = Enum.GetNames(typeof(DeliveryType)).Select(i => i.ToLower());

            var deliveryTypes = _dataService.GetDbSet<Translation>()
                .Where(i => deliveryTypeNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (DeliveryType?)Enum.Parse<DeliveryType>(i.Name, true))
                .ToList();

            return query.Where(i =>
                   i.WarehouseName.ToLower().Contains(search)
                || i.SoldToNumber.ToLower().Contains(search)
                || i.Region.ToLower().Contains(search)
                || i.City.ToLower().Contains(search)
                || i.Address.ToLower().Contains(search)
                || i.PickingFeatures.ToLower().Contains(search)
                //|| i.PickingTypeId != null && pickingTypes.Any(t => t == i.PickingTypeId)
                //|| i.DeliveryType != null && deliveryTypes.Contains(i.DeliveryType)
                || isInt && i.LeadtimeDays == searchInt
                );
        }

        private DetailedValidationResult ValidateDto(WarehouseDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = new DetailedValidationResult();

            if (string.IsNullOrEmpty(dto.SoldToNumber))
            {
                result.AddError(nameof(dto.SoldToNumber), "emptySoldTo".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            if (string.IsNullOrEmpty(dto.WarehouseName))
            {
                result.AddError(nameof(dto.WarehouseName), "emptyWarehouseName".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            if (string.IsNullOrEmpty(dto.DeliveryType))
            {
                result.AddError(nameof(dto.DeliveryType), "emptyDeliveryType".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            var hasDuplicates = _dataService.GetDbSet<Warehouse>()
                                            .Where(x => x.SoldToNumber == dto.SoldToNumber && x.Id.ToString() != dto.Id)
                                            .Any();
            if (hasDuplicates)
            {
                result.AddError(nameof(dto.SoldToNumber), "duplicateSoldTo".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Warehouse, WarehouseDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.PickingTypeId, e => e.MapFrom((s, t) => s.PickingTypeId?.ToString()))
                    .ForMember(t => t.DeliveryType, e => e.MapFrom((s, t) => s.DeliveryType?.ToString()?.ToLowerFirstLetter()));
            });
            return result;
        }
    }
}
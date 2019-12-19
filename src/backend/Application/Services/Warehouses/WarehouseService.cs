using Application.BusinessModels.Shared.Handlers;
using Application.BusinessModels.Warehouses.Handlers;
using Application.Services.Addresses;
using Application.Services.Triggers;
using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using AutoMapper;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
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
        private readonly IChangeTrackerFactory _changeTrackerFactory;

        public WarehousesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, IValidationService validationService,
                                 IHistoryService historyService, ICleanAddressService cleanAddressService, IFieldDispatcherService fieldDispatcherService, IFieldSetterFactory fieldSetterFactory, IChangeTrackerFactory changeTrackerFactory) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
            _cleanAddressService = cleanAddressService;
            _changeTrackerFactory = changeTrackerFactory;
        }

        protected override IFieldSetter<Warehouse> ConfigureHandlers(IFieldSetter<Warehouse> setter, WarehouseDto dto)
        {
            bool isInjection = dto.AdditionalInfo == "INJECTION";

            return setter
                .AddHandler(e => e.WarehouseName, new WarehouseNameHandler(_dataService, _historyService))
                .AddHandler(e => e.Region, new RegionHandler(_dataService, _historyService))
                .AddHandler(e => e.City, new CityHandler(_dataService, _historyService))
                .AddHandler(e => e.Address, new AddressHandler(_dataService, _historyService, _cleanAddressService, !isInjection))
                .AddHandler(e => e.PickingTypeId, new PickingTypeIdHandler(_dataService, _historyService))
                .AddHandler(e => e.LeadtimeDays, new LeadtimeDaysHandler(_dataService, _historyService))
                .AddHandler(e => e.PickingFeatures, new PickingFeaturesHandler(_dataService, _historyService))
                .AddHandler(e => e.AvisaleTime, new AvisaleTimeHandler(_dataService, _historyService))
                .AddHandler(e => e.DeliveryType, new DeliveryTypeHandler(_dataService, _historyService));
        }
        
        protected override IChangeTracker ConfigureChangeTacker()
        {
            return _changeTrackerFactory.CreateChangeTracker()
                .TrackAll<Warehouse>()
                .Remove<Warehouse>(i => i.Id)
                .Remove<Warehouse>(i => i.IsActive);
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

        protected override IEnumerable<WarehouseDto> FillLookupNames(IEnumerable<WarehouseDto> dtos)
        {
            var pickingTypeIds = dtos.Where(x => !string.IsNullOrEmpty(x.PickingTypeId?.Value))
                                     .Select(x => x.PickingTypeId.Value.ToGuid())
                                     .ToList();
            var pickingTypes = _dataService.GetDbSet<PickingType>()
                                           .Where(x => pickingTypeIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.PickingTypeId?.Value)
                    && pickingTypes.TryGetValue(dto.PickingTypeId.Value, out PickingType pickingType))
                {
                    dto.PickingTypeId.Name = pickingType.Name;
                }
                yield return dto;
            }
        }

        public override DetailedValidationResult MapFromDtoToEntity(Warehouse entity, WarehouseDto dto)
        {
            bool isNew = string.IsNullOrEmpty(dto.Id);

            _mapper.Map(dto, entity);
           
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

            //string errors = setter.ValidationErrors
            return null;
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
            return new ExcelMapper<WarehouseDto>(_dataService, _userProvider, _fieldDispatcherService)
                .MapColumn(w => w.PickingTypeId, new DictionaryReferenceExcelColumn(GetPickingTypeIdByName))
                .MapColumn(w => w.DeliveryType, new EnumExcelColumn<DeliveryType>(lang));
        }

        private Guid? GetPickingTypeIdByName(string name)
        {
            var entry = _dataService.GetDbSet<PickingType>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
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

        protected override DetailedValidationResult ValidateDto(WarehouseDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<Warehouse>()
                                            .Where(x => x.SoldToNumber == dto.SoldToNumber && x.Id.ToString() != dto.Id)
                                            .Any();
            if (hasDuplicates)
            {
                result.AddError(nameof(dto.SoldToNumber), "Warehouse.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        private MapperConfiguration ConfigureMapper()
        {
            var lang = _userProvider.GetCurrentUser()?.Language;
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Warehouse, WarehouseDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.PickingTypeId, e => e.MapFrom((s, t) => s.PickingTypeId == null ? null : new LookUpDto(s.PickingTypeId.ToString())))
                    .ForMember(t => t.DeliveryType, e => e.MapFrom((s, t) => s.DeliveryType == null ? null : s.DeliveryType.GetEnumLookup(lang)))
                    .ForMember(t => t.AvisaleTime, e => e.MapFrom((s) => s.AvisaleTime == null ? null : s.AvisaleTime.Value.ToString(@"hh\:mm")));

                cfg.CreateMap<WarehouseDto, Warehouse>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToGuid()))
                    .ForMember(t => t.PickingTypeId, e => e.Condition((s) => s.PickingTypeId != null))
                    .ForMember(t => t.PickingTypeId, e => e.MapFrom((s) => s.PickingTypeId.Value.ToGuid()))
                    .ForMember(t => t.DeliveryType, e => e.Condition((s) => s.DeliveryType != null && !string.IsNullOrEmpty(s.DeliveryType.Value)))
                    .ForMember(t => t.DeliveryType, e => e.MapFrom((s) => MapFromStateDto<DeliveryType>(s.DeliveryType.Value)));
            });
            return result;
        }
    }
}
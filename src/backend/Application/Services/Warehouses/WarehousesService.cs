using Application.BusinessModels.Shared.Handlers;
using Application.BusinessModels.Warehouses.Handlers;
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
using Domain.Services.AppConfiguration;
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
    public class WarehousesService : DictionaryServiceBase<Warehouse, WarehouseDto>, IWarehousesService
    {
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;
        private readonly IChangeTrackerFactory _changeTrackerFactory;

        public WarehousesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, IValidationService validationService,
                                 IHistoryService historyService, IFieldDispatcherService fieldDispatcherService, 
                                 IFieldSetterFactory fieldSetterFactory, IChangeTrackerFactory changeTrackerFactory, IAppConfigurationService configurationService) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, configurationService)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
            _changeTrackerFactory = changeTrackerFactory;
        }

        protected override IFieldSetter<Warehouse> ConfigureHandlers(IFieldSetter<Warehouse> setter, WarehouseDto dto)
        {
            bool isInjection = dto.AdditionalInfo == "INJECTION";

            return setter
                .AddHandler(e => e.Region, new RegionHandler(_dataService, _historyService))
                .AddHandler(e => e.PickingTypeId, new PickingTypeIdHandler(_dataService, _historyService))
                .AddHandler(e => e.LeadtimeDays, new LeadtimeDaysHandler(_dataService, _historyService))
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

        private MapperConfiguration ConfigureMapper()
        {
            var user = _userProvider.GetCurrentUser();

            var lang = user?.Language;
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Warehouse, WarehouseDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.PickingTypeId, e => e.MapFrom((s, t) => s.PickingTypeId == null ? null : new LookUpDto(s.PickingTypeId.ToString())))
                    .ForMember(t => t.ProviderId, e => e.MapFrom((s, t) => s.ProviderId == null ? null : new LookUpDto(s.ProviderId.ToString())))
                    .ForMember(t => t.ClientId, e => e.MapFrom((s, t) => s.ClientId == null ? null : new LookUpDto(s.ClientId.ToString())))
                    .ForMember(t => t.DeliveryType, e => e.MapFrom((s, t) => s.DeliveryType == null ? null : s.DeliveryType.GetEnumLookup(lang)));

                cfg.CreateMap<WarehouseDto, Warehouse>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToGuid()))
                    .ForMember(t => t.PickingTypeId, e => e.Condition((s) => s.PickingTypeId != null))
                    .ForMember(t => t.PickingTypeId, e => e.MapFrom((s) => s.PickingTypeId.Value.ToGuid()))
                    .ForMember(t => t.ProviderId, e => e.Condition((s) => s.ProviderId != null))
                    .ForMember(t => t.ProviderId, e => e.MapFrom((s) => s.ProviderId.Value.ToGuid()))
                    .ForMember(t => t.ClientId, e => e.Condition((s) => s.ClientId != null))
                    .ForMember(t => t.ClientId, e => e.MapFrom((s) => s.ClientId.Value.ToGuid()))
                    .ForMember(t => t.DeliveryType, e => e.Condition((s) => s.DeliveryType != null && !string.IsNullOrEmpty(s.DeliveryType.Value)))
                    .ForMember(t => t.DeliveryType, e => e.MapFrom((s) => MapFromStateDto<DeliveryType>(s.DeliveryType.Value)));
            });
            return result;
        }

        public IEnumerable<LookUpDto> ForSelect(Guid clientId)
        {
            var entities = _dataService.GetDbSet<Warehouse>()
                                       .Where(x => x.IsActive && x.ClientId == clientId)
                                       .OrderBy(x => x.WarehouseName)
                                       .ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Value = entity.Id.ToString(),
                    Name = entity.WarehouseName,
                    Address = entity.Address
                };
            }
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
                    Value = entity.Id.ToString(),
                    Name = entity.WarehouseName,
                    Address = entity.Address
                };
            }
        }

        public override Warehouse FindByKey(WarehouseDto dto)
        {
            var clientId = dto.ClientId?.Value.ToGuid();
            var providerId = dto.ProviderId?.Value.ToGuid();

            return _dataService.GetDbSet<Warehouse>()
                .FirstOrDefault(x => x.Address == dto.Address && x.ClientId == clientId && x.ProviderId == providerId);
        }

        protected override IEnumerable<WarehouseDto> FillLookupNames(IEnumerable<WarehouseDto> dtos)
        {
            var providerIds = dtos.Where(x => !string.IsNullOrEmpty(x.ProviderId?.Value))
                                     .Select(x => x.ProviderId.Value.ToGuid())
                                     .ToList();
            
            var providers = _dataService.GetDbSet<Provider>()
                                           .Where(x => providerIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            var pickingTypeIds = dtos.Where(x => !string.IsNullOrEmpty(x.PickingTypeId?.Value))
                         .Select(x => x.PickingTypeId.Value.ToGuid())
                         .ToList();
            var pickingTypes = _dataService.GetDbSet<PickingType>()
                                           .Where(x => pickingTypeIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            var clinetIds = dtos.Where(x => !string.IsNullOrEmpty(x.ClientId?.Value))
             .Select(x => x.ClientId.Value.ToGuid())
             .ToList();
            var clients = _dataService.GetDbSet<Client>()
                                           .Where(x => clinetIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.PickingTypeId?.Value)
                    && pickingTypes.TryGetValue(dto.PickingTypeId.Value, out PickingType pickingType))
                {
                    dto.PickingTypeId.Name = pickingType.Name;
                }

                if (!string.IsNullOrEmpty(dto.ProviderId?.Value)
                    && providers.TryGetValue(dto.ProviderId.Value, out Provider provider))
                {
                    dto.ProviderId.Name = provider.Name;
                }

                if (!string.IsNullOrEmpty(dto.ClientId?.Value)
                    && clients.TryGetValue(dto.ClientId.Value, out Client client))
                {
                    dto.ClientId.Name = client.Name;
                }

                yield return dto;
            }
        }

        public override DetailedValidationResult MapFromDtoToEntity(Warehouse entity, WarehouseDto dto)
        {
            _mapper.Map(dto, entity);
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
                //.MapColumn(w => w.PickingTypeId, new DictionaryReferenceExcelColumn(GetPickingTypeIdByName))
                .MapColumn(w => w.ProviderId, new DictionaryReferenceExcelColumn(GetProviderIdByName))
                .MapColumn(w => w.ClientId, new DictionaryReferenceExcelColumn(GetClientIdByName))
                //.MapColumn(w => w.DeliveryType, new EnumExcelColumn<DeliveryType>(lang))
                ;
        }
        private Guid? GetProviderIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Provider>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        private Guid? GetClientIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Client>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
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
                || i.Region.ToLower().Contains(search)
                || i.City.ToLower().Contains(search)
                || i.Address.ToLower().Contains(search)
                //|| i.PickingTypeId != null && pickingTypes.Any(t => t == i.PickingTypeId)
                //|| i.DeliveryType != null && deliveryTypes.Contains(i.DeliveryType)
                || isInt && i.LeadtimeDays == searchInt
                );
        }

        public override IQueryable<Warehouse> ApplyRestrictions(IQueryable<Warehouse> query)
        {
            var currentUserId = _userProvider.GetCurrentUserId();
            var user = _dataService.GetById<User>(currentUserId.Value);

            // Local user restrictions

            if (user?.ProviderId != null)
            {
                query = query.Where(i => i.ProviderId == user.ProviderId || i.ProviderId == null);
            }
            
            if (user?.ClientId != null)
            {
                query = query.Where(i => i.ClientId == user.ClientId || i.ClientId == null);
            }

            return query;
        }

        protected override DetailedValidationResult ValidateDto(WarehouseDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && FindByKey(dto) != null;

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Address), "Warehouse.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }
    }
}
using System;
using Application.BusinessModels.Shared.Handlers;
using Application.BusinessModels.ShippingWarehouses.Handlers;
using Application.Services.Addresses;
using Application.Services.Triggers;
using Application.Shared;
using Application.Shared.Excel;
using AutoMapper;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Services.FieldProperties;
using Domain.Services.History;
using Domain.Services.ShippingWarehouses;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System.Collections.Generic;
using System.Linq;
using Application.Shared.Excel.Columns;

namespace Application.Services.ShippingWarehouses
{
    public class ShippingWarehousesService : DictionaryServiceBase<ShippingWarehouse, ShippingWarehouseDto>,
        IShippingWarehousesService
    {
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;
        private readonly ICleanAddressService _cleanAddressService;
        private readonly IChangeTrackerFactory _changeTrackerFactory;


        public ShippingWarehousesService(ICommonDataService dataService, IUserProvider userProvider,
            ITriggersService triggersService, IValidationService validationService,
            IHistoryService historyService, ICleanAddressService cleanAddressService,
            IFieldDispatcherService fieldDispatcherService,
            IFieldSetterFactory fieldSetterFactory, IChangeTrackerFactory changeTrackerFactory,
            IAppConfigurationService configurationService)
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService,
                fieldSetterFactory, configurationService)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
            _cleanAddressService = cleanAddressService;
            _changeTrackerFactory = changeTrackerFactory;
        }

        protected override IFieldSetter<ShippingWarehouse> ConfigureHandlers(IFieldSetter<ShippingWarehouse> setter,
            ShippingWarehouseDto dto)
        {
            return setter
                .AddHandler(e => e.WarehouseName, new ShippingWarehouseNameHandler(_dataService, _historyService))
                .AddHandler(e => e.Address, new AddressHandler(_dataService, _historyService, _cleanAddressService))
                .AddHandler(e => e.City, new CityHandler(_dataService, _historyService));
        }

        protected override IChangeTracker ConfigureChangeTacker()
        {
            return _changeTrackerFactory.CreateChangeTracker().TrackAll<ShippingWarehouse>();
        }

        private MapperConfiguration ConfigureMapper()
        {
            var user = _userProvider.GetCurrentUser();

            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ShippingWarehouse, ShippingWarehouseDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.ProviderId,
                        e => e.MapFrom((s, t) => s.ProviderId == null ? null : new LookUpDto(s.ProviderId.ToString())))
                    .ForMember(t => t.IsEditable,
                        e => e.MapFrom((s, t) => user.ProviderId == null || s.ProviderId != null));

                cfg.CreateMap<ShippingWarehouseDto, ShippingWarehouse>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToGuid()))
                    .ForMember(t => t.ProviderId, e => e.Condition((s) => s.ProviderId != null))
                    .ForMember(t => t.ProviderId, e => e.MapFrom((s) => s.ProviderId.Value.ToGuid()))
                    ;
            });
            return result;
        }

        protected override IEnumerable<ShippingWarehouseDto> FillLookupNames(IEnumerable<ShippingWarehouseDto> dtos)
        {
            var providerIds = dtos.Where(x => !string.IsNullOrEmpty(x.ProviderId?.Value))
                .Select(x => x.ProviderId.Value.ToGuid())
                .ToList();

            var providers = _dataService.GetDbSet<Provider>()
                .Where(x => providerIds.Contains(x.Id))
                .ToDictionary(x => x.Id.ToString());

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.ProviderId?.Value)
                    && providers.TryGetValue(dto.ProviderId.Value, out Provider provider))
                {
                    dto.ProviderId.Name = provider.Name;
                }

                yield return dto;
            }
        }

        public override DetailedValidationResult SaveOrCreate(ShippingWarehouseDto entityFrom)
        {
            var user = _userProvider.GetCurrentUser();

            if (user.ProviderId.HasValue && entityFrom.ProviderId == null)

                entityFrom.ProviderId = new LookUpDto(user.ProviderId.ToString());

            return SaveOrCreateInner(entityFrom, false);
        }

        public ShippingWarehouse GetByCode(string code)
        {
            return _dataService.GetDbSet<ShippingWarehouse>().FirstOrDefault(x => x.Gln == code && x.IsActive);
        }

        public ShippingWarehouseDto GetByNameAndProviderId(string name, Guid providerId)
        {
            return MapFromEntityToDto(_dataService.GetDbSet<ShippingWarehouse>()
                .FirstOrDefault(x => x.WarehouseName == name && x.ProviderId == providerId));
        }

        public Guid? AddColedinoShippingWarehouseToProvider(Guid providerId)
        {
            var isColedinoAllreadyAdded = _dataService.GetDbSet<ShippingWarehouse>()
                .FirstOrDefault(_ => _.ProviderId == providerId && _.WarehouseName.ToLower().Equals("коледино"));
            if (isColedinoAllreadyAdded != null) return null;
            
            var warehouseColedino = new ShippingWarehouse
            {
                Id = Guid.NewGuid(),
                ProviderId = providerId,
                WarehouseName = "Коледино",
                Address = "Московская обл., Подольский р-он с/п Лаговское, вблизи д. Коледино 142181",
                PostalCode = "142181",
                Region = "Московская обл.",
                Area = "Подольский р-он",
                IsActive = true,
                Settlement = "с/п Лаговское, вблизи д. Коледино"
            };
            _dataService.GetDbSet<ShippingWarehouse>().Add(warehouseColedino);
            _dataService.SaveChanges();
            return warehouseColedino.Id;
        }

       public override IEnumerable<LookUpDto> ForSelect(Guid? filter = null)
        {
            var entities = _dataService.GetDbSet<ShippingWarehouse>()
                .Where(x => x.IsActive && (filter == null || x.ProviderId == filter))
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

        public override ShippingWarehouse FindByKey(ShippingWarehouseDto dto)
        {
            return _dataService.GetDbSet<ShippingWarehouse>().FirstOrDefault(x => x.WarehouseName == dto.WarehouseName);
        }

        public override DetailedValidationResult MapFromDtoToEntity(ShippingWarehouse entity, ShippingWarehouseDto dto)
        {
            this._mapper.Map(dto, entity);

            return null;
        }

        public override ShippingWarehouseDto MapFromEntityToDto(ShippingWarehouse entity)
        {
            if (entity == null)
            {
                return null;
            }

            return _mapper.Map<ShippingWarehouseDto>(entity);
        }

        protected override ExcelMapper<ShippingWarehouseDto> CreateExcelMapper()
        {
            return new ExcelMapper<ShippingWarehouseDto>(_dataService, _userProvider, _fieldDispatcherService)
                .MapColumn(w => w.ProviderId, new DictionaryReferenceExcelColumn(GetProviderIdByName));
        }

        private Guid? GetProviderIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Provider>().FirstOrDefault(t => t.Name == name);
            return entry?.Id;
        }

        protected override IQueryable<ShippingWarehouse> ApplySort(IQueryable<ShippingWarehouse> query,
            SearchFormDto form)
        {
            return query
                .OrderBy(i => i.WarehouseName)
                .ThenBy(i => i.Id);
        }

        public override IQueryable<ShippingWarehouse> ApplyRestrictions(IQueryable<ShippingWarehouse> query)
        {
            var currentUserId = _userProvider.GetCurrentUserId();
            var user = _dataService.GetById<User>(currentUserId.Value);

            // Local user restrictions

            if (user?.ProviderId != null)
            {
                query = query.Where(i => i.ProviderId == user.ProviderId);
            }

            return query;
        }

        protected override DetailedValidationResult ValidateDto(ShippingWarehouseDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<ShippingWarehouse>()
                                    .Where(x => x.WarehouseName == dto.WarehouseName && x.Id.ToString() != dto.Id)
                                    .Any();
            if (hasDuplicates)
            {
                result.AddError(nameof(dto.WarehouseName), "ShippingWarehouse.DuplicatedRecord".Translate(lang),
                    ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }
    }
}
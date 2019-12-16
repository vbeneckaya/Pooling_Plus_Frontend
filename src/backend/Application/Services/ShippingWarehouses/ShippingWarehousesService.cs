using Application.BusinessModels.Shared.Handlers;
using Application.BusinessModels.ShippingWarehouses.Handlers;
using Application.Services.Addresses;
using Application.Services.Triggers;
using Application.Shared;
using AutoMapper;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.History;
using Domain.Services.ShippingWarehouses;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.ShippingWarehouses
{
    public class ShippingWarehousesService : DictonaryServiceBase<ShippingWarehouse, ShippingWarehouseDto>, IShippingWarehousesService
    {
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;
        private readonly ICleanAddressService _cleanAddressService;
        private readonly IChangeTrackerFactory _changeTrackerFactory;


        public ShippingWarehousesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, IValidationService validationService,
                                         IHistoryService historyService, ICleanAddressService cleanAddressService, IFieldDispatcherService fieldDispatcherService, IFieldSetterFactory fieldSetterFactory, IChangeTrackerFactory changeTrackerFactory) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
            _cleanAddressService = cleanAddressService;
            _changeTrackerFactory = changeTrackerFactory;
        }

        protected override IFieldSetter<ShippingWarehouse> ConfigureHandlers(IFieldSetter<ShippingWarehouse> setter, ShippingWarehouseDto dto)
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

        public ShippingWarehouse GetByCode(string code)
        {
            return _dataService.GetDbSet<ShippingWarehouse>().FirstOrDefault(x => x.Gln == code && x.IsActive);
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<ShippingWarehouse>().OrderBy(x => x.WarehouseName).ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.WarehouseName,
                    Value = entity.Id.ToString()
                };
            }
        }

        public override ShippingWarehouse FindByKey(ShippingWarehouseDto dto)
        {
            return _dataService.GetDbSet<ShippingWarehouse>().Where(x => x.WarehouseName == dto.WarehouseName).FirstOrDefault();
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

        protected override IQueryable<ShippingWarehouse> ApplySort(IQueryable<ShippingWarehouse> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.WarehouseName)
                .ThenBy(i => i.Id);
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
                result.AddError(nameof(dto.WarehouseName), "ShippingWarehouse.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ShippingWarehouse, ShippingWarehouseDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()));

                cfg.CreateMap<ShippingWarehouseDto, ShippingWarehouse>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToGuid()));
            });
            return result;
        }
    }
}

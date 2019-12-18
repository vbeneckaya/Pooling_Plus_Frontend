﻿using Application.BusinessModels.Shared.Handlers;
using Application.BusinessModels.ShippingWarehouses.Handlers;
using Application.Services.Addresses;
using Application.Services.Triggers;
using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.ShippingWarehouses
{
    public class ShippingWarehousesService : DictionaryServiceBase<ShippingWarehouse, ShippingWarehouseDto>, IShippingWarehousesService
    {
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;
        private readonly ICleanAddressService _cleanAddressService;
        private readonly IChangeTrackerFactory _changeTrackerFactory;


        public ShippingWarehousesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, IValidationService validationService,
                                         IHistoryService historyService, ICleanAddressService cleanAddressService, IFieldDispatcherService fieldDispatcherService, 
                                         IFieldSetterFactory fieldSetterFactory, IChangeTrackerFactory changeTrackerFactory, IAppConfigurationService configurationService) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, configurationService)
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
            return _changeTrackerFactory.CreateChangeTracker()
                .Add<ShippingWarehouse>(i => i.Code)
                .Add<ShippingWarehouse>(i => i.WarehouseName)
                .Add<ShippingWarehouse>(i => i.Address)
                .Add<ShippingWarehouse>(i => i.Region)
                .Add<ShippingWarehouse>(i => i.City);
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ShippingWarehouse, ShippingWarehouseDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.CompanyId, e => e.MapFrom((s, t) => s.CompanyId == null ? null : new LookUpDto(s.CompanyId.ToString())));

                cfg.CreateMap<ShippingWarehouseDto, ShippingWarehouse>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToGuid()))
                    .ForMember(t => t.CompanyId, e => e.Condition((s) => s.CompanyId != null))
                    .ForMember(t => t.CompanyId, e => e.MapFrom((s) => s.CompanyId.Value.ToGuid()));
            });
            return result;
        }

        protected override IEnumerable<ShippingWarehouseDto> FillLookupNames(IEnumerable<ShippingWarehouseDto> dtos)
        {
            var companyIds = dtos.Where(x => !string.IsNullOrEmpty(x.CompanyId?.Value))
                                     .Select(x => x.CompanyId.Value.ToGuid())
                                     .ToList();

            var companies = _dataService.GetDbSet<Company>()
                                           .Where(x => companyIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.CompanyId?.Value)
                    && companies.TryGetValue(dto.CompanyId.Value, out Company company))
                {
                    dto.CompanyId.Name = company.Name;
                }

                yield return dto;
            }
        }

        public ShippingWarehouse GetByCode(string code)
        {
            return _dataService.GetDbSet<ShippingWarehouse>().FirstOrDefault(x => x.Code == code && x.IsActive);
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
            return _dataService.GetDbSet<ShippingWarehouse>().Where(x => x.Code == dto.Code).FirstOrDefault();
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
                .MapColumn(w => w.CompanyId, new DictionaryReferenceExcelColumn(GetCompanyIdByName));
        }

        private Guid? GetCompanyIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Company>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        protected override IQueryable<ShippingWarehouse> ApplySort(IQueryable<ShippingWarehouse> query, SearchFormDto form)
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

            if (user?.CompanyId != null)
            {
                query = query.Where(i => i.CompanyId == user.CompanyId || i.CompanyId == null);
            }

            return query;
        }

        protected override DetailedValidationResult ValidateDto(ShippingWarehouseDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<ShippingWarehouse>()
                                            .Where(x => x.Code == dto.Code && x.Id.ToString() != dto.Id)
                                            .Any();
            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Code), "ShippingWarehouse.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override UserConfigurationDictionaryItem GetDictionaryConfiguration(Guid id)
        {
            var entity = _dataService.GetById<ShippingWarehouse>(id);

            var configuration = base.GetDictionaryConfiguration(id);

            var companyId = configuration.Columns.First(i => i.Name.ToLower() == nameof(ShippingWarehouse.CompanyId).ToLower());
            companyId.IsReadOnly = entity.CompanyId != null;

            return configuration;
        }
    }
}

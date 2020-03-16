using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Services.FieldProperties;
using Domain.Services.Tariffs;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query.Internal;
using OfficeOpenXml;
using Serilog;

namespace Application.Services.Tariffs
{
    public class TariffsService : DictionaryServiceBase<Tariff, TariffDto>, ITariffsService
    {
        private readonly IMapper _mapper;
        
        public TariffsService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                              IValidationService validationService, IFieldDispatcherService fieldDispatcherService, 
                              IFieldSetterFactory fieldSetterFactory, IAppConfigurationService configurationService) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, configurationService)
        {
            _mapper = ConfigureMapper().CreateMapper();
        }
        
        public virtual ImportResultDto ImportFromExcel(Stream fileStream)
        {
            string entityName = typeof(Tariff).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var excel = new ExcelPackage(fileStream);
            var workSheet = excel.Workbook.Worksheets[0]; //.ElementAt(0);

            var excelMapper = CreateExcelMapper();
            
            var dtos = excelMapper.LoadEntries(workSheet);

            var providers = dtos.Select(_ => Guid.Parse(_.Data.ProviderId.Value)).Distinct().ToList();
            
            var validShippingWarehouses =
                _dataService.GetDbSet<ShippingWarehouse>().Where(x=>providers.Contains(x.ProviderId));
            
            dtos = dtos.Select(x=> CheckShippingWarehouseProvider(x, validShippingWarehouses));
            
            Log.Information("{entityName}.ImportFromExcel (Load from file): {ElapsedMilliseconds}ms", entityName,
                sw.ElapsedMilliseconds);
            sw.Restart();

            var importResult = Import(dtos);
            Log.Information("{entityName}.ImportFromExcel (Import): {ElapsedMilliseconds}ms", entityName,
                sw.ElapsedMilliseconds);

            return importResult;
        }

        private ValidatedRecord<TariffDto> CheckShippingWarehouseProvider(ValidatedRecord<TariffDto> dto, IQueryable<ShippingWarehouse> validShippingWarehouses)
        {
            dto.Data.ShippingWarehouseId.Value = validShippingWarehouses.FirstOrDefault(_ =>
                _.ProviderId == Guid.Parse(dto.Data.ProviderId.Value)
                && _.WarehouseName == dto.Data.ShippingWarehouseId.Name).Id.ToString();
            return dto;
        }

        public override  DetailedValidationResult SaveOrCreate(TariffDto entityFrom)
        {
            var user = _userProvider.GetCurrentUser();
            
            if (user.ProviderId.HasValue && entityFrom.ProviderId == null)
                
                entityFrom.ProviderId = new LookUpDto(user.ProviderId.ToString());
            
            return SaveOrCreateInner(entityFrom, false);
        }
        
        public override DetailedValidationResult MapFromDtoToEntity(Tariff entity, TariffDto dto)
        {
            entity = _mapper.Map<Tariff>(dto);

            return null;
        }
        
        protected override DetailedValidationResult ValidateDto(TariffDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            var result = base.ValidateDto(dto); //new DetailedValidationResult();

            // Delivery Warehouse

            if (!_dataService.GetDbSet<Warehouse>().Any(i => Guid.Empty != i.Id && i.Id == Guid.Parse(dto.DeliveryWarehouseId.Value)))
            {
                result.AddError(nameof(dto.DeliveryWarehouseId), "Tariff.DeliveryWarehouse.InvalidDictionaryValue".Translate(lang), ValidationErrorType.InvalidDictionaryValue);
            }

            // Shipping Warehouse

            if (!_dataService.GetDbSet<ShippingWarehouse>().Any(i => Guid.Empty != i.Id && i.Id  == Guid.Parse(dto.ShippingWarehouseId.Value)))
            {
                result.AddError(nameof(dto.ShippingWarehouseId), "Tariff.ShipmentWarehouse.InvalidDictionaryValue".Translate(lang), ValidationErrorType.InvalidDictionaryValue);
            }
            
            // Provider

            if (!_dataService.GetDbSet<Provider>().Any(i => Guid.Empty != i.Id && i.Id  == Guid.Parse(dto.ProviderId.Value)))
            {
                result.AddError(nameof(dto.ProviderId), "Tariff.Provider.InvalidDictionaryValue".Translate(lang), ValidationErrorType.InvalidDictionaryValue);
            }

            var existingRecord = this.GetByKey(dto)
                .Where(i => i.Id != dto.Id.ToGuid())
                .Where(i => IsPeriodsOverlapped(i, dto));

            var hasDuplicates = !result.IsError && existingRecord.Any();

            if (hasDuplicates)
            {
                result.AddError("duplicateTariffs", "Tariff.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        private bool IsPeriodsOverlapped(Tariff tariff, TariffDto dto)
        {
            var expirationDate = dto.ExpirationDate.ToDate().GetValueOrDefault(DateTime.MaxValue);
            var effectiveDate = dto.EffectiveDate.ToDate().GetValueOrDefault(DateTime.MinValue);

            return !(expirationDate <= tariff.EffectiveDate.GetValueOrDefault(DateTime.MaxValue) && effectiveDate <= tariff.EffectiveDate.GetValueOrDefault(DateTime.MaxValue)
                || effectiveDate >= tariff.ExpirationDate.GetValueOrDefault(DateTime.MinValue) && expirationDate >= tariff.ExpirationDate.GetValueOrDefault(DateTime.MinValue));
        }
        
        protected override IEnumerable<TariffDto> FillLookupNames(IEnumerable<TariffDto> dtos)
        {
            var carrierIds = dtos.Where(x => !string.IsNullOrEmpty(x.CarrierId?.Value))
                                 .Select(x => x.CarrierId.Value.ToGuid())
                                 .ToList();
            var carriers = _dataService.GetDbSet<TransportCompany>()
                                       .Where(x => carrierIds.Contains(x.Id))
                                       .ToDictionary(x => x.Id.ToString());
            var providerIds = dtos.Where(x => !string.IsNullOrEmpty(x.ProviderId?.Value))
                .Select(x => x.ProviderId.Value.ToGuid())
                .ToList();
            var providers = _dataService.GetDbSet<Provider>()
                .Where(x => providerIds.Contains(x.Id))
                .ToDictionary(x => x.Id.ToString());

            var vehicleTypeIds = dtos.Where(x => !string.IsNullOrEmpty(x.VehicleTypeId?.Value))
                                     .Select(x => x.VehicleTypeId.Value.ToGuid())
                                     .ToList();
            var vehicleTypes = _dataService.GetDbSet<VehicleType>()
                                           .Where(x => vehicleTypeIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            var bodyTypeIds = dtos.Where(x => !string.IsNullOrEmpty(x.BodyTypeId?.Value))
                                  .Select(x => x.BodyTypeId.Value.ToGuid())
                                  .ToList();
            var bodyTypes = _dataService.GetDbSet<BodyType>()
                                        .Where(x => bodyTypeIds.Contains(x.Id))
                                        .ToDictionary(x => x.Id.ToString());

            var deliveryWarehouseIds = dtos.Where(x => !string.IsNullOrEmpty(x.DeliveryWarehouseId?.Value))
                .Select(x => x.DeliveryWarehouseId.Value.ToGuid())
                .ToList();
            
            var deliveryWarehouses = _dataService.GetDbSet<Warehouse>()
                .Where(x => deliveryWarehouseIds.Contains(x.Id))
                .ToDictionary(x => x.Id.ToString());
            
            var shippingWarehouseIds = dtos.Where(x => !string.IsNullOrEmpty(x.ShippingWarehouseId?.Value))
                .Select(x => x.ShippingWarehouseId.Value.ToGuid())
                .Distinct()
                .ToList();

            var shippingWarehouses = _dataService.GetDbSet<ShippingWarehouse>()
                .Where(x => shippingWarehouseIds.Contains(x.Id))
                .ToDictionary(x => x.Id.ToString());
            

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.CarrierId?.Value)
                    && carriers.TryGetValue(dto.CarrierId.Value, out TransportCompany carrier))
                {
                    dto.CarrierId.Name = carrier.Title;
                }
                
                if (!string.IsNullOrEmpty(dto.ProviderId?.Value)
                    && providers.TryGetValue(dto.ProviderId.Value, out Provider provider))
                {
                    dto.ProviderId.Name = provider.Name;
                }

                if (!string.IsNullOrEmpty(dto.VehicleTypeId?.Value)
                    && vehicleTypes.TryGetValue(dto.VehicleTypeId.Value, out VehicleType vehicleType))
                {
                    dto.VehicleTypeId.Name = vehicleType.Name;
                }

                if (!string.IsNullOrEmpty(dto.BodyTypeId?.Value)
                    && bodyTypes.TryGetValue(dto.BodyTypeId.Value, out BodyType bodyType))
                {
                    dto.BodyTypeId.Name = bodyType.Name;
                }

                if (!string.IsNullOrEmpty(dto.ShippingWarehouseId?.Value)
                    && shippingWarehouses.TryGetValue(dto.ShippingWarehouseId.Value, out ShippingWarehouse shippingWarehouse))
                {
                    dto.ShippingWarehouseId.Name = shippingWarehouse.WarehouseName;
                }
                
                if (!string.IsNullOrEmpty(dto.DeliveryWarehouseId?.Value)
                    && deliveryWarehouses.TryGetValue(dto.DeliveryWarehouseId.Value, out Warehouse deliveryWarehouse))
                {
                    dto.DeliveryWarehouseId.Name = deliveryWarehouse.WarehouseName;
                }
                
                yield return dto;
            }
        }

        public override TariffDto MapFromEntityToDto(Tariff entity)
        {
            return _mapper.Map<TariffDto>(entity);
        }

        protected override ExcelMapper<TariffDto> CreateExcelMapper()
        {
            return new ExcelMapper<TariffDto>(_dataService, _userProvider, _fieldDispatcherService)
                .MapColumn(w => w.CarrierId, new DictionaryReferenceExcelColumn(GetCarrierIdByName))
                .MapColumn(w => w.ProviderId, new DictionaryReferenceExcelColumn(GetProviderIdByName))
                .MapColumn(w => w.DeliveryWarehouseId, new DictionaryReferenceExcelColumn(GetDeliveryWarehouseIdByName))
                .MapColumn(w => w.ShippingWarehouseId, new DictionaryReferenceExcelColumn(GetShippingWarehouseIdByName))
                .MapColumn(w => w.VehicleTypeId, new DictionaryReferenceExcelColumn(GetVehicleTypeIdByName));
        }

        private Guid? GetCarrierIdByName(string name)
        {
            var entry = _dataService.GetDbSet<TransportCompany>().FirstOrDefault(t => t.Title == name);
            return entry?.Id;
        }

        private Guid? GetProviderIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Provider>().FirstOrDefault(t => t.Name == name);
            return entry?.Id;
        }
        
        private Guid? GetDeliveryWarehouseIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Warehouse>().FirstOrDefault(t => t.WarehouseName == name);
            return entry?.Id;
        }
        
        private Guid? GetShippingWarehouseIdByName(string name)
        {
            var entry = _dataService.GetDbSet<ShippingWarehouse>().FirstOrDefault(t => t.WarehouseName == name);
            return entry?.Id;
        }

        private Guid? GetVehicleTypeIdByName(string name)
        {
            var entry = _dataService.GetDbSet<VehicleType>().FirstOrDefault(t => t.Name == name);
            return entry?.Id;
        }

//        private Guid? GetBodyTypeIdByName(string name)
//        {
//            var entry = _dataService.GetDbSet<BodyType>().Where(t => t.Name == name).FirstOrDefault();
//            return entry?.Id;
//        }

        protected override IQueryable<Tariff> ApplySearch(IQueryable<Tariff> query, SearchFormDto form)
        {
            if (string.IsNullOrEmpty(form.Search)) return query;

            var search = form.Search.ToLower();

            var transportCompanies = this._dataService.GetDbSet<TransportCompany>()
                .Where(i => i.Title.ToLower().Contains(search))
                .Select(i => i.Id);
            
            var providers = this._dataService.GetDbSet<Provider>()
                .Where(i => i.Name.ToLower().Contains(search))
                .Select(i => i.Id);
            
            var shippingWarehouses = this._dataService.GetDbSet<ShippingWarehouse>()
                .Where(i => i.WarehouseName.ToLower().Contains(search))
                .Select(i => i.Id);
            
            var deliveryWarehouses = this._dataService.GetDbSet<Warehouse>()
                .Where(i => i.WarehouseName.ToLower().Contains(search))
                .Select(i => i.Id);

            var vehicleTypes = this._dataService.GetDbSet<VehicleType>()
                .Where(i => i.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                .Select(i => i.Id).ToList();

//            var tarifficationTypeNames = Enum.GetNames(typeof(TarifficationType)).Select(i => i.ToLower());

//            var tarifficationTypes = this._dataService.GetDbSet<Translation>()
//                .Where(i => tarifficationTypeNames.Contains(i.Name.ToLower()))
//                .WhereTranslation(search)
//                .Select(i => (TarifficationType?)Enum.Parse<TarifficationType>(i.Name, true))
//                .ToList();

            var searchDateFormat = "dd.MM.yyyy HH:mm";

            return query.Where(i =>
                   transportCompanies.Any(t => t == i.CarrierId)
                || vehicleTypes.Any(t => t == i.VehicleTypeId)
                || i.StartWinterPeriod.HasValue && i.StartWinterPeriod.Value.ToString(searchDateFormat).Contains(search)
                || i.EndWinterPeriod.HasValue && i.EndWinterPeriod.Value.ToString(searchDateFormat).Contains(search)
                || providers.Contains(i.ProviderId.Value)
                || shippingWarehouses.Contains(i.ShippingWarehouseId)
                || deliveryWarehouses.Contains(i.DeliveryWarehouseId)
                );
        }

        public override IQueryable<Tariff> ApplyRestrictions(IQueryable<Tariff> query)
        {
            var currentUserId = _userProvider.GetCurrentUserId();
            var user = _dataService.GetById<User>(currentUserId.Value);

            // Local user restrictions

            if (user?.ProviderId != null)
            {
                query = query.Where(i => i.ProviderId == user.ProviderId || i.ProviderId == null);
            }

            return query;
        }
        
        public override Tariff FindByKey(TariffDto dto)
        {
            var effectiveDate = dto.EffectiveDate.ToDate();
            var expirationDate = dto.ExpirationDate.ToDate();

            return GetByKey(dto)
                .FirstOrDefault(i => i.EffectiveDate == effectiveDate &&
                                     i.ExpirationDate == expirationDate);
        }

        private IQueryable<Tariff> GetByKey(TariffDto dto)
        {
            var carrierId = dto.CarrierId?.Value?.ToGuid();
            var vehicleTypeId = dto.VehicleTypeId?.Value?.ToGuid();
            var shipmentWarehouse = dto.ShippingWarehouseId?.Value;
            var deliveryWarehouse = dto.DeliveryWarehouseId?.Value;
            var provider = dto.ProviderId?.Value;
            return _dataService.GetDbSet<Tariff>()
                    .Where(i =>
                        i.CarrierId == carrierId
                        && i.VehicleTypeId == vehicleTypeId
                        && Guid.Empty != i.ShippingWarehouseId && i.ShippingWarehouseId == Guid.Parse(shipmentWarehouse)
                        && Guid.Empty != i.DeliveryWarehouseId && i.DeliveryWarehouseId == Guid.Parse(deliveryWarehouse)
                        && Guid.Empty != i.ProviderId && i.ProviderId == Guid.Parse(provider)
                    );
        }
        
         private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Tariff, TariffDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    
                    .ForMember(t => t.ProviderId, e => e.MapFrom((s, t) => s.ProviderId == null ? null : new LookUpDto(s.ProviderId.ToString())))
                    .ForMember(t => t.CarrierId, e => e.MapFrom((s, t) => s.CarrierId == null ? null : new LookUpDto(s.CarrierId.ToString())))
                    .ForMember(t => t.ShippingWarehouseId, e => e.MapFrom((s, t) => s.ShippingWarehouseId == null ? null : new LookUpDto(s.ShippingWarehouseId.ToString())))
                    .ForMember(t => t.DeliveryWarehouseId, e => e.MapFrom((s, t) => s.DeliveryWarehouseId == null ? null : new LookUpDto(s.DeliveryWarehouseId.ToString())))
                    .ForMember(t => t.VehicleTypeId, e => e.MapFrom((s, t) => s.VehicleTypeId == null ? null : new LookUpDto(s.VehicleTypeId.ToString())))
                    .ForMember(t => t.BodyTypeId, e => e.MapFrom((s, t) => s.BodyTypeId == null ? null : new LookUpDto(s.BodyTypeId.ToString())))
                    .ForMember(t => t.EffectiveDate, e => e.MapFrom((s, t) => s.EffectiveDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ExpirationDate, e => e.MapFrom((s, t) => s.ExpirationDate?.ToString("dd.MM.yyyy")))
                    ;

                cfg.CreateMap<TariffDto, Tariff>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToGuid()))

                    .ForMember(t => t.ProviderId, e => e.MapFrom((s) => s.ProviderId == null ? null : s.ProviderId.Value.ToGuid()))
                    .ForMember(t => t.CarrierId, e => e.MapFrom((s) =>  s.CarrierId == null ? null : s.CarrierId.Value.ToGuid()))
                    .ForMember(t => t.ShippingWarehouseId, e => e.MapFrom((s) =>  s.ShippingWarehouseId == null ? null : s.ShippingWarehouseId.Value.ToGuid()))
                    .ForMember(t => t.DeliveryWarehouseId, e => e.MapFrom((s) => s.DeliveryWarehouseId == null ? null : s.DeliveryWarehouseId.Value.ToGuid()))
                    .ForMember(t => t.VehicleTypeId, e => e.MapFrom((s) => s.VehicleTypeId == null ? null : s.VehicleTypeId.Value.ToGuid()))
                    .ForMember(t => t.BodyTypeId, e => e.MapFrom((s) => s.BodyTypeId == null ? null : s.BodyTypeId.Value.ToGuid()))
                    .ForMember(t => t.EffectiveDate, e => e.MapFrom((s) => s.EffectiveDate.ToDate()))
                    .ForMember(t => t.ExpirationDate, e => e.MapFrom((s) => s.ExpirationDate.ToDate()))
                    ;
            });
            return result;
        }
   }
}
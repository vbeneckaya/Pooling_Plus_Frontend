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
using System.Globalization;
using System.Linq;

namespace Application.Services.Tariffs
{
    public class TariffsService : DictionaryServiceBase<Tariff, TariffDto>, ITariffsService
    {
        public TariffsService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                              IValidationService validationService, IFieldDispatcherService fieldDispatcherService, 
                              IFieldSetterFactory fieldSetterFactory, IAppConfigurationService configurationService) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, configurationService)
        {
        }

        public override DetailedValidationResult MapFromDtoToEntity(Tariff entity, TariffDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);

            entity.ShipmentCity = dto.ShipmentCity?.Value;
            entity.DeliveryCity = dto.DeliveryCity?.Value;
            entity.TarifficationType = string.IsNullOrEmpty(dto.TarifficationType?.Value) ? (TarifficationType?)null : MapFromStateDto<TarifficationType>(dto.TarifficationType.Value);
            entity.VehicleTypeId = dto.VehicleTypeId?.Value?.ToGuid();
            entity.CompanyId = dto.CompanyId?.Value?.ToGuid();
            entity.CarrierId = dto.CarrierId?.Value?.ToGuid();
            entity.BodyTypeId = dto.BodyTypeId?.Value?.ToGuid();
            entity.WinterAllowance = dto.WinterAllowance.ToDecimal();
            entity.EffectiveDate = dto.EffectiveDate.ToDate();
            entity.ExpirationDate = dto.ExpirationDate.ToDate();

            if (dto.StartWinterPeriod.TryParseDate(out DateTime swPeriod))
            {
                entity.StartWinterPeriod = swPeriod;
            }
            else
            {
                entity.StartWinterPeriod = null;
            }

            if (dto.EndWinterPeriod.TryParseDate(out DateTime ewPeriod))
            {
                entity.EndWinterPeriod = ewPeriod;
            }
            else
            {
                entity.EndWinterPeriod = null;
            }

            entity.FtlRate = dto.FtlRate;
            entity.LtlRate1 = dto.LtlRate1;
            entity.LtlRate2 = dto.LtlRate2;
            entity.LtlRate3 = dto.LtlRate3;
            entity.LtlRate4 = dto.LtlRate4;
            entity.LtlRate5 = dto.LtlRate5;
            entity.LtlRate6 = dto.LtlRate6;
            entity.LtlRate7 = dto.LtlRate7;
            entity.LtlRate8 = dto.LtlRate8;
            entity.LtlRate9 = dto.LtlRate9;
            entity.LtlRate10 = dto.LtlRate10;
            entity.LtlRate11 = dto.LtlRate11;
            entity.LtlRate12 = dto.LtlRate12;
            entity.LtlRate13 = dto.LtlRate13;
            entity.LtlRate14 = dto.LtlRate14;
            entity.LtlRate15 = dto.LtlRate15;
            entity.LtlRate16 = dto.LtlRate16;
            entity.LtlRate17 = dto.LtlRate17;
            entity.LtlRate18 = dto.LtlRate18;
            entity.LtlRate19 = dto.LtlRate19;
            entity.LtlRate20 = dto.LtlRate20;
            entity.LtlRate21 = dto.LtlRate21;
            entity.LtlRate22 = dto.LtlRate22;
            entity.LtlRate23 = dto.LtlRate23;
            entity.LtlRate24 = dto.LtlRate24;
            entity.LtlRate25 = dto.LtlRate25;
            entity.LtlRate26 = dto.LtlRate26;
            entity.LtlRate27 = dto.LtlRate27;
            entity.LtlRate28 = dto.LtlRate28;
            entity.LtlRate29 = dto.LtlRate29;
            entity.LtlRate30 = dto.LtlRate30;
            entity.LtlRate31 = dto.LtlRate31;
            entity.LtlRate32 = dto.LtlRate32;
            entity.LtlRate33 = dto.LtlRate33;

            return null;
        }

        protected override DetailedValidationResult ValidateDto(TariffDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            var result = base.ValidateDto(dto); //new DetailedValidationResult();

            // Delivery City

            if (!_dataService.GetDbSet<Warehouse>().Any(i => !string.IsNullOrEmpty(i.City) && i.City.ToLower() == dto.DeliveryCity.Value.ToLower()))
            {
                result.AddError(nameof(dto.DeliveryCity), "Tariff.DeliveryCity.InvalidDictionaryValue".Translate(lang), ValidationErrorType.InvalidDictionaryValue);
            }

            // Shipping City

            if (!_dataService.GetDbSet<ShippingWarehouse>().Any(i => !string.IsNullOrEmpty(i.City) && i.City.ToLower() == dto.ShipmentCity.Value.ToLower()))
            {
                result.AddError(nameof(dto.ShipmentCity), "Tariff.ShipmentCity.InvalidDictionaryValue".Translate(lang), ValidationErrorType.InvalidDictionaryValue);
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

        // From ValidationService
        private bool IsDateValid(string dateString)
        {
            return string.IsNullOrEmpty(dateString) || dateString.ToDate().HasValue;
        }

        protected override IEnumerable<TariffDto> FillLookupNames(IEnumerable<TariffDto> dtos)
        {
            var carrierIds = dtos.Where(x => !string.IsNullOrEmpty(x.CarrierId?.Value))
                                 .Select(x => x.CarrierId.Value.ToGuid())
                                 .ToList();
            var carriers = _dataService.GetDbSet<TransportCompany>()
                                       .Where(x => carrierIds.Contains(x.Id))
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

            var companyIds = dtos.Where(x => !string.IsNullOrEmpty(x.CompanyId?.Value))
                         .Select(x => x.CompanyId.Value.ToGuid())
                         .ToList();

            var companies = _dataService.GetDbSet<Company>()
                                           .Where(x => companyIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.CarrierId?.Value)
                    && carriers.TryGetValue(dto.CarrierId.Value, out TransportCompany carrier))
                {
                    dto.CarrierId.Name = carrier.Title;
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

                if (!string.IsNullOrEmpty(dto.CompanyId?.Value)
                    && companies.TryGetValue(dto.CompanyId.Value, out Company company))
                {
                    dto.CompanyId.Name = company.Name;
                }

                yield return dto;
            }
        }

        public override TariffDto MapFromEntityToDto(Tariff entity)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;
            return new TariffDto
            {
                Id = entity.Id.ToString(),
                ShipmentCity = string.IsNullOrEmpty(entity.ShipmentCity) ? null : new LookUpDto(entity.ShipmentCity),
                DeliveryCity = string.IsNullOrEmpty(entity.DeliveryCity) ? null : new LookUpDto(entity.DeliveryCity),
                TarifficationType = entity.TarifficationType == null ? null : entity.TarifficationType.GetEnumLookup(lang),
                CarrierId = entity.CarrierId == null ? null : new LookUpDto(entity.CarrierId.ToString()),
                VehicleTypeId = entity.VehicleTypeId == null ? null : new LookUpDto(entity.VehicleTypeId.ToString()),
                CompanyId = entity.CompanyId == null ? null : new LookUpDto(entity.CompanyId.ToString()),
                BodyTypeId = entity.BodyTypeId == null ? null : new LookUpDto(entity.BodyTypeId.ToString()),
                StartWinterPeriod = entity.StartWinterPeriod?.ToString("dd.MM.yyyy"),
                EndWinterPeriod = entity.EndWinterPeriod?.ToString("dd.MM.yyyy"),
                WinterAllowance = entity.WinterAllowance.HasValue ? 
                    entity.WinterAllowance.Value.ToString("F3", CultureInfo.InvariantCulture) : null,
                EffectiveDate = entity.EffectiveDate?.ToString("dd.MM.yyyy"),
                ExpirationDate = entity.ExpirationDate?.ToString("dd.MM.yyyy"),
                FtlRate = entity.FtlRate,
                LtlRate1 = entity.LtlRate1,
                LtlRate2 = entity.LtlRate2,
                LtlRate3 = entity.LtlRate3,
                LtlRate4 = entity.LtlRate4,
                LtlRate5 = entity.LtlRate5,
                LtlRate6 = entity.LtlRate6,
                LtlRate7 = entity.LtlRate7,
                LtlRate8 = entity.LtlRate8,
                LtlRate9 = entity.LtlRate9,
                LtlRate10 = entity.LtlRate10,
                LtlRate11 = entity.LtlRate11,
                LtlRate12 = entity.LtlRate12,
                LtlRate13 = entity.LtlRate13,
                LtlRate14 = entity.LtlRate14,
                LtlRate15 = entity.LtlRate15,
                LtlRate16 = entity.LtlRate16,
                LtlRate17 = entity.LtlRate17,
                LtlRate18 = entity.LtlRate18,
                LtlRate19 = entity.LtlRate19,
                LtlRate20 = entity.LtlRate20,
                LtlRate21 = entity.LtlRate21,
                LtlRate22 = entity.LtlRate22,
                LtlRate23 = entity.LtlRate23,
                LtlRate24 = entity.LtlRate24,
                LtlRate25 = entity.LtlRate25,
                LtlRate26 = entity.LtlRate26,
                LtlRate27 = entity.LtlRate27,
                LtlRate28 = entity.LtlRate28,
                LtlRate29 = entity.LtlRate29,
                LtlRate30 = entity.LtlRate30,
                LtlRate31 = entity.LtlRate31,
                LtlRate32 = entity.LtlRate32,
                LtlRate33 = entity.LtlRate33,
                /*end of map entity to dto fields*/
            };
        }

        protected override ExcelMapper<TariffDto> CreateExcelMapper()
        {
            string lang = _userProvider.GetCurrentUser()?.Language;
            return new ExcelMapper<TariffDto>(_dataService, _userProvider, _fieldDispatcherService)
                .MapColumn(w => w.TarifficationType, new EnumExcelColumn<TarifficationType>(lang))
                .MapColumn(w => w.CarrierId, new DictionaryReferenceExcelColumn(GetCarrierIdByName))
                .MapColumn(w => w.CompanyId, new DictionaryReferenceExcelColumn(GetCompanyIdByName))
                .MapColumn(w => w.VehicleTypeId, new DictionaryReferenceExcelColumn(GetVehicleTypeIdByName))
                .MapColumn(w => w.BodyTypeId, new DictionaryReferenceExcelColumn(GetBodyTypeIdByName));
        }

        private Guid? GetCarrierIdByName(string name)
        {
            var entry = _dataService.GetDbSet<TransportCompany>().Where(t => t.Title == name).FirstOrDefault();
            return entry?.Id;
        }

        private Guid? GetCompanyIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Company>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        private Guid? GetVehicleTypeIdByName(string name)
        {
            var entry = _dataService.GetDbSet<VehicleType>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        private Guid? GetBodyTypeIdByName(string name)
        {
            var entry = _dataService.GetDbSet<BodyType>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        protected override IQueryable<Tariff> ApplySearch(IQueryable<Tariff> query, SearchFormDto form)
        {
            if (string.IsNullOrEmpty(form.Search)) return query;

            var search = form.Search.ToLower();

            var transportCompanies = this._dataService.GetDbSet<TransportCompany>()
                .Where(i => i.Title.ToLower().Contains(search))
                .Select(i => i.Id);

            var vehicleTypes = this._dataService.GetDbSet<VehicleType>()
                .Where(i => i.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                .Select(i => i.Id).ToList();

            var tarifficationTypeNames = Enum.GetNames(typeof(TarifficationType)).Select(i => i.ToLower());

            var tarifficationTypes = this._dataService.GetDbSet<Translation>()
                .Where(i => tarifficationTypeNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (TarifficationType?)Enum.Parse<TarifficationType>(i.Name, true))
                .ToList();

            var searchDateFormat = "dd.MM.yyyy HH:mm";

            return query.Where(i =>
                   transportCompanies.Any(t => t == i.CarrierId)
                || vehicleTypes.Any(t => t == i.VehicleTypeId)
                || tarifficationTypes.Contains(i.TarifficationType)
                || i.StartWinterPeriod.HasValue && i.StartWinterPeriod.Value.ToString(searchDateFormat).Contains(search)
                || i.EndWinterPeriod.HasValue && i.EndWinterPeriod.Value.ToString(searchDateFormat).Contains(search)
                || tarifficationTypes.Contains(i.TarifficationType)
                || i.ShipmentCity.ToLower().Contains(search)
                || i.DeliveryCity.ToLower().Contains(search)
                );
        }

        public override IQueryable<Tariff> ApplyRestrictions(IQueryable<Tariff> query)
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

        public override Tariff FindByKey(TariffDto dto)
        {
            var effectiveDate = dto.EffectiveDate.ToDate();
            var expirationDate = dto.ExpirationDate.ToDate();

            return this.GetByKey(dto)
                .Where(i => i.EffectiveDate == effectiveDate)
                .Where(i => i.ExpirationDate == expirationDate)
                .FirstOrDefault();
        }

        private IQueryable<Tariff> GetByKey(TariffDto dto)
        {
            Guid? carrierId = dto.CarrierId?.Value?.ToGuid();
            Guid? vehicleTypeId = dto.VehicleTypeId?.Value?.ToGuid();
            Guid? bodyTypeId = dto.BodyTypeId?.Value?.ToGuid();
            string shipmentCity = dto.ShipmentCity?.Value;
            string deliveryCity = dto.DeliveryCity?.Value;
            TarifficationType? tarifficationType = dto.TarifficationType?.Value?.Parse<TarifficationType>();
            return _dataService.GetDbSet<Tariff>()
                    .Where(i =>
                        i.CarrierId == carrierId
                        && i.VehicleTypeId == vehicleTypeId
                        && i.BodyTypeId == bodyTypeId
                        && i.TarifficationType == tarifficationType
                        && !string.IsNullOrEmpty(i.ShipmentCity) && i.ShipmentCity == shipmentCity
                        && !string.IsNullOrEmpty(i.DeliveryCity) && i.DeliveryCity == deliveryCity);
        }

        public override UserConfigurationDictionaryItem GetDictionaryConfiguration(Guid id)
        {
            var entity = _dataService.GetById<Tariff>(id);

            var configuration = base.GetDictionaryConfiguration(id);

            var companyId = configuration.Columns.First(i => i.Name.ToLower() == nameof(Tariff.CompanyId).ToLower());
            companyId.IsReadOnly = entity.CompanyId != null;

            return configuration;
        }
    }
}
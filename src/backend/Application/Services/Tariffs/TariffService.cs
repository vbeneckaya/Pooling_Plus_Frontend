using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using DAL.Queries;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Tariffs;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using Serilog;
using System;
using System.Globalization;
using System.Linq;

namespace Application.Services.Tariffs
{
    public class TariffsService : DictonaryServiceBase<Tariff, TariffDto>, ITariffsService
    {
        public TariffsService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public override ValidateResult MapFromDtoToEntity(Tariff entity, TariffDto dto)
        {
            var validateResult = ValidateDto(dto);
            if (validateResult.IsError)
            {
                return validateResult;
            }

            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);

            entity.ShipmentCity = dto.ShipmentCity;
            entity.DeliveryCity = dto.DeliveryCity;
            entity.TarifficationType = dto.TarifficationType.Parse<TarifficationType>();
            entity.VehicleTypeId = dto.VehicleTypeId.ToGuid();
            entity.CarrierId = dto.CarrierId.ToGuid();
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

            return new ValidateResult(null, entity.Id.ToString());
        }

        private ValidateResult ValidateDto(TariffDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidattionResult result = new DetailedValidattionResult();

            if (string.IsNullOrEmpty(dto.ShipmentCity))
            {
                result.AddError(nameof(dto.ShipmentCity), "emptyShipmentCity".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            if (string.IsNullOrEmpty(dto.DeliveryCity))
            {
                result.AddError(nameof(dto.DeliveryCity), "emptyDeliveryCity".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            if (string.IsNullOrEmpty(dto.CarrierId))
            {
                result.AddError(nameof(dto.CarrierId), "emptyCarrierId".Translate(lang), ValidationErrorType.ValueIsRequired);
            }


            if (string.IsNullOrEmpty(dto.StartWinterPeriod))
            {
                result.AddError(nameof(dto.StartWinterPeriod), "emptyStartWinterPeriod".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            if (string.IsNullOrEmpty(dto.EndWinterPeriod))
            {
                result.AddError(nameof(dto.EndWinterPeriod), "emptyEndWinterPeriod".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            if (string.IsNullOrEmpty(dto.EffectiveDate))
            {
                result.AddError(nameof(dto.EffectiveDate), "emptyEffectiveDate".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            if (string.IsNullOrEmpty(dto.ExpirationDate))
            {
                result.AddError(nameof(dto.ExpirationDate), "emptyExpirationDate".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            var existingRecord = this.GetByKey(dto)
                .Where(i => i.Id != dto.Id.ToGuid())
                .FirstOrDefault();

            var hasDuplicates = existingRecord != null && IsPeriodsOverlapped(existingRecord, dto);

            if (hasDuplicates)
            {
                result.AddError("duplicateTariffs", "duplicateTariffs".Translate(lang), ValidationErrorType.DuplicatedRecord);
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

        public override TariffDto MapFromEntityToDto(Tariff entity)
        {
            return new TariffDto
            {
                Id = entity.Id.ToString(),
                ShipmentCity = entity.ShipmentCity,
                DeliveryCity = entity.DeliveryCity,
                TarifficationType = entity.TarifficationType?.ToString().ToLowerFirstLetter(),
                CarrierId = entity.CarrierId?.ToString(),
                VehicleTypeId = entity.VehicleTypeId?.ToString(),
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
            return new ExcelMapper<TariffDto>(_dataService, _userProvider)
                .MapColumn(w => w.TarifficationType, new EnumExcelColumn<TarifficationType>())
                .MapColumn(w => w.CarrierId, new DictionaryReferenceExcelColumn(GetCarrierIdByName, GetCarrierNameById))
                .MapColumn(w => w.VehicleTypeId, new DictionaryReferenceExcelColumn(GetVehicleTypeIdByName, GetVehicleTypeNameById));
        }

        private Guid? GetCarrierIdByName(string name)
        {
            var entry = _dataService.GetDbSet<TransportCompany>().Where(t => t.Title == name).FirstOrDefault();
            return entry?.Id;
        }

        private string GetCarrierNameById(Guid id)
        {
            var entry = _dataService.GetDbSet<TransportCompany>().Find(id);
            return entry?.Title;
        }

        private Guid? GetVehicleTypeIdByName(string name)
        {
            var entry = _dataService.GetDbSet<VehicleType>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        private string GetVehicleTypeNameById(Guid id)
        {
            var entry = _dataService.GetDbSet<VehicleType>().GetById(id);
            return entry?.Name;
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

        public Tariff FindByKeyExt(TariffDto dto)
        {
            var effectiveDate = dto.EffectiveDate.ToDate();
            var expirationDate = dto.ExpirationDate.ToDate();

            return this.GetByKey(dto)
                .Where(i => i.EffectiveDate == effectiveDate)
                .Where(i => i.ExpirationDate == expirationDate)
                .FirstOrDefault();
        }

        private bool HasDatesOverlapped(TariffDto dto)
        {
            var list = this.GetByKey(dto).Where(i => IsPeriodsOverlapped(i, dto)).ToList();

            return this.GetByKey(dto).Any(i => IsPeriodsOverlapped(i, dto));
        }

        public override Tariff FindByKey(TariffDto dto)
        {
            return GetByKey(dto).FirstOrDefault();
        }

        private IQueryable<Tariff> GetByKey(TariffDto dto)
        {
            return _dataService.GetDbSet<Tariff>()
                    .Where(i =>
                        i.CarrierId == dto.CarrierId.ToGuid()
                        && i.VehicleTypeId == dto.VehicleTypeId.ToGuid()
                        && i.TarifficationType == dto.TarifficationType.Parse<TarifficationType>()
                        && !string.IsNullOrEmpty(i.ShipmentCity) && i.ShipmentCity == dto.ShipmentCity
                        && !string.IsNullOrEmpty(i.DeliveryCity) && i.DeliveryCity == dto.DeliveryCity);
        }

        protected override ValidateResult SaveOrCreateInner(TariffDto entityFrom, bool isImport)
        {
            var lang = this._userProvider.GetCurrentUser().Language;

            var dbSet = _dataService.GetDbSet<Tariff>();

            Tariff entityFromDb = null;

            if (!isImport)
            {
                entityFromDb = FindByKey(entityFrom);
            }
            else 
            {
                entityFromDb = FindByKeyExt(entityFrom);

                if (entityFromDb == null)
                {
                    if (HasDatesOverlapped(entityFrom))
                        return new DetailedValidattionResult("duplicated", "tariffs.duplicated".Translate(lang), ValidationErrorType.DuplicatedRecord);
                }
            }

            // Rest of SaveOrCreateInner logic

            var isNew = entityFromDb == null;

            if (isNew)
            {
                entityFromDb = new Tariff
                {
                    Id = Guid.NewGuid()
                };
            }
            else if (isImport)
            {
                entityFrom.Id = entityFromDb.Id.ToString();
            }

            var result = MapFromDtoToEntity(entityFromDb, entityFrom);

            if (!result.IsError)
            {
                if (isNew)
                {
                    dbSet.Add(entityFromDb);
                    result.ResultType = ValidateResultType.Created;
                }
                else
                {
                    dbSet.Update(entityFromDb);
                    result.ResultType = ValidateResultType.Updated;
                }

                _dataService.SaveChanges();

                Log.Information($"«апись {entityFromDb.Id} в справочнике {typeof(Tariff)} {(isNew ? "создана" : "обновлена")}.");
            }
            else
            {
                Log.Information($"Ќе удалось сохранить запись в справочник {typeof(Tariff)}: {result.Error}.");
            }

            return result;
        }
    }
}
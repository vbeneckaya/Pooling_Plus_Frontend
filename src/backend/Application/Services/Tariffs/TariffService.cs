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
using System;
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

            var existingRecord = this.FindByKey(dto);
            var hasDuplicates = existingRecord != null && existingRecord.Id != dto.Id.ToGuid();

            if (hasDuplicates)
            {
                result.AddError("duplicateTariffs", "duplicateTariffs".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
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
            var entry = _dataService.GetDbSet<TransportCompany>().GetById(id);
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

            return query.Where(i =>
                   transportCompanies.Any(t => t == i.CarrierId)
                || vehicleTypes.Any(t => t == i.VehicleTypeId)
                || tarifficationTypes.Contains(i.TarifficationType)
                || i.ShipmentCity.ToLower().Contains(search)
                || i.DeliveryCity.ToLower().Contains(search)
                );
        }

        public override Tariff FindByKey(TariffDto dto)
        {
            return _dataService.GetDbSet<Tariff>()
                .Where(i =>
                    i.CarrierId == dto.CarrierId.ToGuid()
                    && i.VehicleTypeId == dto.VehicleTypeId.ToGuid()
                    && i.TarifficationType == dto.TarifficationType.Parse<TarifficationType>()
                    && !string.IsNullOrEmpty(i.ShipmentCity) && i.ShipmentCity == dto.ShipmentCity
                    && !string.IsNullOrEmpty(i.DeliveryCity) && i.DeliveryCity == dto.DeliveryCity)
                .FirstOrDefault();
        }
    }
}
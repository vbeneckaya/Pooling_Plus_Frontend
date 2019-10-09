using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Extensions;
using Domain.Services.Tariffs;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using System.Linq;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using DAL.Queries;

namespace Application.Services.Tariffs
{
    public class TariffsService : DictonaryServiceBase<Tariff, TariffDto>, ITariffsService
    {
        public TariffsService(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override DbSet<Tariff> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Tariffs;
        }

        public override void MapFromDtoToEntity(Tariff entity, TariffDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.ShipmentCity = dto.ShipmentCity;
            entity.DeliveryCity = dto.DeliveryCity;
            entity.TarifficationType = string.IsNullOrEmpty(dto.TarifficationType) ? (TarifficationType?)null : Enum.Parse<TarifficationType>(dto.TarifficationType.ToUpperfirstLetter());
            entity.CarrierId = string.IsNullOrEmpty(dto.CarrierId) ? (Guid?)null : Guid.Parse(dto.CarrierId);
            entity.VehicleTypeId = string.IsNullOrEmpty(dto.VehicleTypeId) ? (Guid?)null : Guid.Parse(dto.VehicleTypeId);
            entity.FTLRate = dto.FTLRate;
            entity.LTLRate1 = dto.LTLRate1;
            entity.LTLRate2 = dto.LTLRate2;
            entity.LTLRate3 = dto.LTLRate3;
            entity.LTLRate4 = dto.LTLRate4;
            entity.LTLRate5 = dto.LTLRate5;
            entity.LTLRate6 = dto.LTLRate6;
            entity.LTLRate7 = dto.LTLRate7;
            entity.LTLRate8 = dto.LTLRate8;
            entity.LTLRate9 = dto.LTLRate9;
            entity.LTLRate10 = dto.LTLRate10;
            entity.LTLRate11 = dto.LTLRate11;
            entity.LTLRate12 = dto.LTLRate12;
            entity.LTLRate13 = dto.LTLRate13;
            entity.LTLRate14 = dto.LTLRate14;
            entity.LTLRate15 = dto.LTLRate15;
            entity.LTLRate16 = dto.LTLRate16;
            entity.LTLRate17 = dto.LTLRate17;
            entity.LTLRate18 = dto.LTLRate18;
            entity.LTLRate19 = dto.LTLRate19;
            entity.LTLRate20 = dto.LTLRate20;
            entity.LTLRate21 = dto.LTLRate21;
            entity.LTLRate22 = dto.LTLRate22;
            entity.LTLRate23 = dto.LTLRate23;
            entity.LTLRate24 = dto.LTLRate24;
            entity.LTLRate25 = dto.LTLRate25;
            entity.LTLRate26 = dto.LTLRate26;
            entity.LTLRate27 = dto.LTLRate27;
            entity.LTLRate28 = dto.LTLRate28;
            entity.LTLRate29 = dto.LTLRate29;
            entity.LTLRate30 = dto.LTLRate30;
            entity.LTLRate31 = dto.LTLRate31;
            entity.LTLRate32 = dto.LTLRate32;
            entity.LTLRate33 = dto.LTLRate33;
            /*end of map dto to entity fields*/
        }

        public override TariffDto MapFromEntityToDto(Tariff entity)
        {
            return new TariffDto
            {
                Id = entity.Id.ToString(),
                ShipmentCity = entity.ShipmentCity,
                DeliveryCity = entity.DeliveryCity,
                TarifficationType = entity.TarifficationType?.ToString().ToLowerfirstLetter(),
                CarrierId = entity.CarrierId?.ToString(),
                VehicleTypeId = entity.VehicleTypeId?.ToString(),
                FTLRate = entity.FTLRate,
                LTLRate1 = entity.LTLRate1,
                LTLRate2 = entity.LTLRate2,
                LTLRate3 = entity.LTLRate3,
                LTLRate4 = entity.LTLRate4,
                LTLRate5 = entity.LTLRate5,
                LTLRate6 = entity.LTLRate6,
                LTLRate7 = entity.LTLRate7,
                LTLRate8 = entity.LTLRate8,
                LTLRate9 = entity.LTLRate9,
                LTLRate10 = entity.LTLRate10,
                LTLRate11 = entity.LTLRate11,
                LTLRate12 = entity.LTLRate12,
                LTLRate13 = entity.LTLRate13,
                LTLRate14 = entity.LTLRate14,
                LTLRate15 = entity.LTLRate15,
                LTLRate16 = entity.LTLRate16,
                LTLRate17 = entity.LTLRate17,
                LTLRate18 = entity.LTLRate18,
                LTLRate19 = entity.LTLRate19,
                LTLRate20 = entity.LTLRate20,
                LTLRate21 = entity.LTLRate21,
                LTLRate22 = entity.LTLRate22,
                LTLRate23 = entity.LTLRate23,
                LTLRate24 = entity.LTLRate24,
                LTLRate25 = entity.LTLRate25,
                LTLRate26 = entity.LTLRate26,
                LTLRate27 = entity.LTLRate27,
                LTLRate28 = entity.LTLRate28,
                LTLRate29 = entity.LTLRate29,
                LTLRate30 = entity.LTLRate30,
                LTLRate31 = entity.LTLRate31,
                LTLRate32 = entity.LTLRate32,
                LTLRate33 = entity.LTLRate33,
                /*end of map entity to dto fields*/
            };
        }

        protected override ExcelMapper<TariffDto> CreateExcelMapper()
        {
            return new ExcelMapper<TariffDto>(db)
                .MapColumn(w => w.TarifficationType, new EnumExcelColumn<TarifficationType>())
                .MapColumn(w => w.CarrierId, new DictionaryReferenceExcelColumn(GetCarrierIdByName, GetCarrierNameById))
                .MapColumn(w => w.VehicleTypeId, new DictionaryReferenceExcelColumn(GetVehicleTypeIdByName, GetVehicleTypeNameById));
        }

        private Guid? GetCarrierIdByName(string name)
        {
            var entry = db.TransportCompanies.Where(t => t.Title == name).FirstOrDefault();
            return entry?.Id;
        }

        private string GetCarrierNameById(Guid id)
        {
            var entry = db.TransportCompanies.GetById(id);
            return entry?.Title;
        }

        private Guid? GetVehicleTypeIdByName(string name)
        {
            var entry = db.VehicleTypes.Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        private string GetVehicleTypeNameById(Guid id)
        {
            var entry = db.VehicleTypes.GetById(id);
            return entry?.Name;
        }
    }
}
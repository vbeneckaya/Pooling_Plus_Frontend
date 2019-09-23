using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Extensions;
using Domain.Services.Tariffs;
using Microsoft.EntityFrameworkCore;

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
            entity.CityOfShipment = dto.CityOfShipment;
            entity.DeliveryCity = dto.DeliveryCity;
            entity.BillingMethod = dto.BillingMethod;
            entity.TransportCompany = dto.TransportCompany;
            entity.VehicleType = dto.VehicleType;
            entity.FTLBet = dto.FTLBet;
            entity.LTLRate1 = dto.LTLRate1;
            entity.LTLRate2 = dto.LTLRate2;
            entity.BetLTL3 = dto.BetLTL3;
            entity.LTLRate4 = dto.LTLRate4;
            entity.LTLRate5 = dto.LTLRate5;
            entity.LTLRate6 = dto.LTLRate6;
            entity.LTLRate7 = dto.LTLRate7;
            entity.LTLBet8 = dto.LTLBet8;
            entity.LTLRate9 = dto.LTLRate9;
            entity.LTLRate10 = dto.LTLRate10;
            entity.LTLRate11 = dto.LTLRate11;
            entity.LTLRate12 = dto.LTLRate12;
            entity.LTLRate13 = dto.LTLRate13;
            entity.LTLRate14 = dto.LTLRate14;
            entity.BetLTL15 = dto.BetLTL15;
            entity.LTLRate16 = dto.LTLRate16;
            entity.BetLTL17 = dto.BetLTL17;
            entity.BetLTL18 = dto.BetLTL18;
            entity.LTLRate19 = dto.LTLRate19;
            entity.LTLRate20 = dto.LTLRate20;
            entity.LTLRate21 = dto.LTLRate21;
            entity.LTLRate22 = dto.LTLRate22;
            entity.LTLRate23 = dto.LTLRate23;
            entity.LTLRate24 = dto.LTLRate24;
            entity.LTLRate25 = dto.LTLRate25;
            entity.LTLBet26 = dto.LTLBet26;
            entity.LTLRate27 = dto.LTLRate27;
            entity.LTLBet28 = dto.LTLBet28;
            entity.LTLRate29 = dto.LTLRate29;
            entity.LTLRate30 = dto.LTLRate30;
            entity.LTLRate31 = dto.LTLRate31;
            entity.BetLTL32 = dto.BetLTL32;
            entity.LTLBid33 = dto.LTLBid33;
            /*end of map dto to entity fields*/
        }

        public override TariffDto MapFromEntityToDto(Tariff entity)
        {
            return new TariffDto
            {
                Id = entity.Id.ToString(),
                CityOfShipment = entity.CityOfShipment,
                DeliveryCity = entity.DeliveryCity,
                BillingMethod = entity.BillingMethod,
                TransportCompany = entity.TransportCompany,
                VehicleType = entity.VehicleType,
                FTLBet = entity.FTLBet,
                LTLRate1 = entity.LTLRate1,
                LTLRate2 = entity.LTLRate2,
                BetLTL3 = entity.BetLTL3,
                LTLRate4 = entity.LTLRate4,
                LTLRate5 = entity.LTLRate5,
                LTLRate6 = entity.LTLRate6,
                LTLRate7 = entity.LTLRate7,
                LTLBet8 = entity.LTLBet8,
                LTLRate9 = entity.LTLRate9,
                LTLRate10 = entity.LTLRate10,
                LTLRate11 = entity.LTLRate11,
                LTLRate12 = entity.LTLRate12,
                LTLRate13 = entity.LTLRate13,
                LTLRate14 = entity.LTLRate14,
                BetLTL15 = entity.BetLTL15,
                LTLRate16 = entity.LTLRate16,
                BetLTL17 = entity.BetLTL17,
                BetLTL18 = entity.BetLTL18,
                LTLRate19 = entity.LTLRate19,
                LTLRate20 = entity.LTLRate20,
                LTLRate21 = entity.LTLRate21,
                LTLRate22 = entity.LTLRate22,
                LTLRate23 = entity.LTLRate23,
                LTLRate24 = entity.LTLRate24,
                LTLRate25 = entity.LTLRate25,
                LTLBet26 = entity.LTLBet26,
                LTLRate27 = entity.LTLRate27,
                LTLBet28 = entity.LTLBet28,
                LTLRate29 = entity.LTLRate29,
                LTLRate30 = entity.LTLRate30,
                LTLRate31 = entity.LTLRate31,
                BetLTL32 = entity.BetLTL32,
                LTLBid33 = entity.LTLBid33,
                /*end of map entity to dto fields*/
            };
        }
    }
}
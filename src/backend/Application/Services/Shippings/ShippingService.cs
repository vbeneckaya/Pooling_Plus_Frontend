using System;
using System.Collections.Generic;
using Application.BusinessModels.Shippings.Actions;
using Application.Shared;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Shippings;
using Domain.Services.UserIdProvider;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Shippings
{
    public class ShippingsService : GridWithDocumentsBase<Shipping, ShippingDto, ShippingDto>, IShippingsService
    {
        public ShippingsService(AppDbContext appDbContext, IUserIdProvider userIdProvider) : base(appDbContext, userIdProvider)
        {
        }

        public override DbSet<Shipping> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Shippings;
        }

        public override IEnumerable<IAction<Shipping>> Actions()
        {
            return new List<IAction<Shipping>>
            {
                new CancelShipping(db),
                new SendShippingToTk(db),
                new CancelRequestShipping(db),
                new ConfirmShipping(db),
                /*end of add single actions*/
            };
        }
        
        public override LookUpDto MapFromEntityToLookupDto(Shipping entity)
        {
            return new LookUpDto
            {
                Value = entity.Id.ToString(),
                Name = entity.ShippingNumber
            };
        }

        public override IEnumerable<IAction<IEnumerable<Shipping>>> GroupActions()
        {
            return new List<IAction<IEnumerable<Shipping>>>
            {
                /*end of add group actions*/
            };
        }

        public override ValidateResult MapFromDtoToEntity(Shipping entity, ShippingDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.ShippingNumber = dto.ShippingNumber;
            if (!string.IsNullOrEmpty(dto.DeliveryType))
                entity.DeliveryType = MapFromStateDto<DeliveryType>(dto.DeliveryType);
            entity.TemperatureMin = dto.TemperatureMin;
            entity.TemperatureMax = dto.TemperatureMax;
            if (!string.IsNullOrEmpty(dto.TarifficationType))
                entity.TarifficationType = MapFromStateDto<TarifficationType>(dto.TarifficationType);
            entity.Carrier = dto.Carrier;
            entity.VehicleType = dto.VehicleType;
            entity.PalletsCount = dto.PalletsCount;
            entity.ActualPalletsCount = dto.ActualPalletsCount;
            entity.ConfirmedPalletsCount = dto.ConfirmedPalletsCount;
            entity.WeightKg = dto.WeightKg;
            entity.ActualWeightKg = dto.ActualWeightKg;
            entity.PlannedArrivalTimeSlotBDFWarehouse = dto.PlannedArrivalTimeSlotBDFWarehouse;
            entity.LoadingArrivalTime = dto.LoadingArrivalTime;
            entity.LoadingDepartureTime = dto.LoadingDepartureTime;
            entity.DeliveryInvoiceNumber = dto.DeliveryInvoiceNumber;
            entity.DeviationReasonsComments = dto.DeviationReasonsComments;
            entity.TotalDeliveryCost = dto.TotalDeliveryCost;
            entity.OtherCosts = dto.OtherCosts;
            entity.DeliveryCostWithoutVAT = dto.DeliveryCostWithoutVAT;
            entity.ReturnCostWithoutVAT = dto.ReturnCostWithoutVAT;
            entity.InvoiceAmountWithoutVAT = dto.InvoiceAmountWithoutVAT;
            entity.AdditionalCostsWithoutVAT = dto.AdditionalCostsWithoutVAT;
            entity.AdditionalCostsComments = dto.AdditionalCostsComments;
            entity.TrucksDowntime = dto.TrucksDowntime;
            entity.ReturnRate = dto.ReturnRate;
            entity.AdditionalPointRate = dto.AdditionalPointRate;
            entity.DowntimeRate = dto.DowntimeRate;
            entity.BlankArrivalRate = dto.BlankArrivalRate;
            entity.BlankArrival = dto.BlankArrival ?? false;
            entity.Waybill = dto.Waybill ?? false;
            entity.WaybillTorg12 = dto.WaybillTorg12 ?? false;
            entity.TransportWaybill = dto.TransportWaybill ?? false;
            entity.Invoice = dto.Invoice ?? false;
            entity.DocumentsReturnDate = dto.DocumentsReturnDate;
            entity.ActualDocumentsReturnDate = dto.ActualDocumentsReturnDate;
            entity.InvoiceNumber = dto.InvoiceNumber;
            if(!string.IsNullOrEmpty(dto.Status))
                entity.Status =  MapFromStateDto<ShippingState>(dto.Status);
            entity.CostsConfirmedByShipper = dto.CostsConfirmedByShipper ?? false;
            entity.CostsConfirmedByCarrier = dto.CostsConfirmedByCarrier ?? false;
            /*end of map dto to entity fields*/

            return new ValidateResult(null, entity.Id.ToString());
        }

        public override ValidateResult MapFromFormDtoToEntity(Shipping entity, ShippingDto dto)
        {
            return MapFromDtoToEntity(entity, dto);
        }

        public override ShippingDto MapFromEntityToDto(Shipping entity)
        {
            return new ShippingDto
            {
                Id = entity.Id.ToString(),
                ShippingNumber = entity.ShippingNumber,
                DeliveryType = entity.DeliveryType.ToString().ToLowerfirstLetter(),
                TemperatureMin = entity.TemperatureMin,
                TemperatureMax = entity.TemperatureMax,
                TarifficationType = entity.TarifficationType.ToString().ToLowerfirstLetter(),
                Carrier = entity.Carrier,
                VehicleType = entity.VehicleType,
                PalletsCount = entity.PalletsCount,
                ActualPalletsCount = entity.ActualPalletsCount,
                ConfirmedPalletsCount = entity.ConfirmedPalletsCount,
                WeightKg = entity.WeightKg,
                ActualWeightKg = entity.ActualWeightKg,
                PlannedArrivalTimeSlotBDFWarehouse = entity.PlannedArrivalTimeSlotBDFWarehouse,
                LoadingArrivalTime = entity.LoadingArrivalTime,
                LoadingDepartureTime = entity.LoadingDepartureTime,
                DeliveryInvoiceNumber = entity.DeliveryInvoiceNumber,
                DeviationReasonsComments = entity.DeviationReasonsComments,
                TotalDeliveryCost = entity.TotalDeliveryCost,
                OtherCosts = entity.OtherCosts,
                DeliveryCostWithoutVAT = entity.DeliveryCostWithoutVAT,
                ReturnCostWithoutVAT = entity.ReturnCostWithoutVAT,
                InvoiceAmountWithoutVAT = entity.InvoiceAmountWithoutVAT,
                AdditionalCostsWithoutVAT = entity.AdditionalCostsWithoutVAT,
                AdditionalCostsComments = entity.AdditionalCostsComments,
                TrucksDowntime = entity.TrucksDowntime,
                ReturnRate = entity.ReturnRate,
                AdditionalPointRate = entity.AdditionalPointRate,
                DowntimeRate = entity.DowntimeRate,
                BlankArrivalRate = entity.BlankArrivalRate,
                BlankArrival = entity.BlankArrival,
                Waybill = entity.Waybill,
                WaybillTorg12 = entity.WaybillTorg12,
                TransportWaybill = entity.TransportWaybill,
                Invoice = entity.Invoice,
                DocumentsReturnDate = entity.DocumentsReturnDate,
                ActualDocumentsReturnDate = entity.ActualDocumentsReturnDate,
                InvoiceNumber = entity.InvoiceNumber,
                Status = entity.Status.ToString().ToLowerfirstLetter(),
                CostsConfirmedByShipper = entity.CostsConfirmedByShipper,
                CostsConfirmedByCarrier = entity.CostsConfirmedByCarrier,
                /*end of map entity to dto fields*/
            };
        }

        public override ShippingDto MapFromEntityToFormDto(Shipping entity)
        {
            return MapFromEntityToDto(entity);
        }
    }
}
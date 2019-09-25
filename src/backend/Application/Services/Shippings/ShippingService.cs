using System;
using System.Collections.Generic;
using Application.Actions.Shippings;
using Application.Shared;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Shippings;
using Domain.Services.UserIdProvider;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Shippings
{
    public class ShippingsService : GridServiceBase<Shipping, ShippingDto>, IShippingsService
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
                /*end of add single actions*/
            };
        }

        public override IEnumerable<IAction<IEnumerable<Shipping>>> GroupActions()
        {
            return new List<IAction<IEnumerable<Shipping>>>
            {
                /*end of add group actions*/
            };
        }

        public override void MapFromDtoToEntity(Shipping entity, ShippingDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.TransportationNumber = dto.TransportationNumber;
            entity.DeliveryMethod = dto.DeliveryMethod;
            entity.ThermalMode = dto.ThermalMode;
            entity.BillingMethod = dto.BillingMethod;
            entity.TransportCompany = dto.TransportCompany;
            entity.PreliminaryNumberOfPallets = dto.PreliminaryNumberOfPallets;
            entity.ActualNumberOfPallets = dto.ActualNumberOfPallets;
            entity.ConfirmedNumberOfPallets = dto.ConfirmedNumberOfPallets;
            entity.PlannedArrivalTimeSlotBDFWarehouse = dto.PlannedArrivalTimeSlotBDFWarehouse;
            entity.ArrivalTimeForLoadingBDFWarehouse = dto.ArrivalTimeForLoadingBDFWarehouse;
            entity.DepartureTimeFromTheBDFWarehouse = dto.DepartureTimeFromTheBDFWarehouse;
            entity.DeliveryInvoiceNumber = dto.DeliveryInvoiceNumber;
            entity.CommentsReasonsForDeviationFromTheSchedule = dto.CommentsReasonsForDeviationFromTheSchedule;
            entity.TransportationCostWithoutVAT = dto.TransportationCostWithoutVAT;
            entity.ReturnShippingCostExcludingVAT = dto.ReturnShippingCostExcludingVAT;
            entity.AdditionalShippingCostsExcludingVAT = dto.AdditionalShippingCostsExcludingVAT;
            entity.AdditionalShippingCostsComments = dto.AdditionalShippingCostsComments;
            entity.Waybill = dto.Waybill;
            entity.WaybillTorg12 = dto.WaybillTorg12;
            entity.WaybillTransportSection = dto.WaybillTransportSection;
            entity.Invoice = dto.Invoice;
            entity.ActualReturnDate = dto.ActualReturnDate;
            entity.InvoiceNumber = dto.InvoiceNumber;
            if(!string.IsNullOrEmpty(dto.Status))
                entity.Status =  MapFromStateDto<ShippingState>(dto.Status);
            entity.DeliveryStatus = dto.DeliveryStatus;
            entity.AmountConfirmedByShipper = dto.AmountConfirmedByShipper;
            entity.AmountConfirmedByTC = dto.AmountConfirmedByTC;
            /*end of map dto to entity fields*/
        }

        public override ShippingDto MapFromEntityToDto(Shipping entity)
        {
            return new ShippingDto
            {
                Id = entity.Id.ToString(),
                TransportationNumber = entity.TransportationNumber,
                DeliveryMethod = entity.DeliveryMethod,
                ThermalMode = entity.ThermalMode,
                BillingMethod = entity.BillingMethod,
                TransportCompany = entity.TransportCompany,
                PreliminaryNumberOfPallets = entity.PreliminaryNumberOfPallets,
                ActualNumberOfPallets = entity.ActualNumberOfPallets,
                ConfirmedNumberOfPallets = entity.ConfirmedNumberOfPallets,
                PlannedArrivalTimeSlotBDFWarehouse = entity.PlannedArrivalTimeSlotBDFWarehouse,
                ArrivalTimeForLoadingBDFWarehouse = entity.ArrivalTimeForLoadingBDFWarehouse,
                DepartureTimeFromTheBDFWarehouse = entity.DepartureTimeFromTheBDFWarehouse,
                DeliveryInvoiceNumber = entity.DeliveryInvoiceNumber,
                CommentsReasonsForDeviationFromTheSchedule = entity.CommentsReasonsForDeviationFromTheSchedule,
                TransportationCostWithoutVAT = entity.TransportationCostWithoutVAT,
                ReturnShippingCostExcludingVAT = entity.ReturnShippingCostExcludingVAT,
                AdditionalShippingCostsExcludingVAT = entity.AdditionalShippingCostsExcludingVAT,
                AdditionalShippingCostsComments = entity.AdditionalShippingCostsComments,
                Waybill = entity.Waybill,
                WaybillTorg12 = entity.WaybillTorg12,
                WaybillTransportSection = entity.WaybillTransportSection,
                Invoice = entity.Invoice,
                ActualReturnDate = entity.ActualReturnDate,
                InvoiceNumber = entity.InvoiceNumber,
                Status = entity.Status.ToString().ToLowerfirstLetter(),
                DeliveryStatus = entity.DeliveryStatus,
                AmountConfirmedByShipper = entity.AmountConfirmedByShipper,
                AmountConfirmedByTC = entity.AmountConfirmedByTC,
                /*end of map entity to dto fields*/
            };
        }
    }
}
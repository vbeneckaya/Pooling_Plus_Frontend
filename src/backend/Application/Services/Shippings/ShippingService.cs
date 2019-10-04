using System;
using System.Collections.Generic;
using Application.BusinessModels.Shippings.Actions;
using Application.BusinessModels.Shippings.Handlers;
using Application.Shared;
using AutoMapper;
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
            _mapper = ConfigureMapper().CreateMapper();
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
            var setter = new FieldSetter<Shipping>(entity);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id));
            setter.UpdateField(e => e.ShippingNumber, dto.ShippingNumber);
            setter.UpdateField(e => e.DeliveryType, string.IsNullOrEmpty(dto.DeliveryType) ? (DeliveryType?)null : MapFromStateDto<DeliveryType>(dto.DeliveryType));
            setter.UpdateField(e => e.TemperatureMin, dto.TemperatureMin);
            setter.UpdateField(e => e.TemperatureMax, dto.TemperatureMax);
            setter.UpdateField(e => e.TarifficationType, string.IsNullOrEmpty(dto.TarifficationType) ? (TarifficationType?)null : MapFromStateDto<TarifficationType>(dto.TarifficationType));
            setter.UpdateField(e => e.CarrierId, string.IsNullOrEmpty(dto.CarrierId) ? (Guid?)null : Guid.Parse(dto.CarrierId));
            setter.UpdateField(e => e.VehicleTypeId, string.IsNullOrEmpty(dto.VehicleTypeId) ? (Guid?)null : Guid.Parse(dto.VehicleTypeId));
            setter.UpdateField(e => e.PalletsCount, dto.PalletsCount, new PalletsCountHandler());
            setter.UpdateField(e => e.ActualPalletsCount, dto.ActualPalletsCount, new ActualPalletsCountHandler());
            setter.UpdateField(e => e.ConfirmedPalletsCount, dto.ConfirmedPalletsCount, new ConfirmedPalletsCountHandler());
            setter.UpdateField(e => e.WeightKg, dto.WeightKg, new WeightKgHandler());
            setter.UpdateField(e => e.ActualWeightKg, dto.ActualWeightKg, new ActualWeightKgHandler());
            setter.UpdateField(e => e.PlannedArrivalTimeSlotBDFWarehouse, dto.PlannedArrivalTimeSlotBDFWarehouse);
            setter.UpdateField(e => e.LoadingArrivalTime, ParseDateTime(dto.LoadingArrivalTime));
            setter.UpdateField(e => e.LoadingDepartureTime, ParseDateTime(dto.LoadingDepartureTime));
            setter.UpdateField(e => e.DeliveryInvoiceNumber, dto.DeliveryInvoiceNumber);
            setter.UpdateField(e => e.DeviationReasonsComments, dto.DeviationReasonsComments);
            setter.UpdateField(e => e.TotalDeliveryCost, dto.TotalDeliveryCost, new TotalDeliveryCostHandler());
            setter.UpdateField(e => e.OtherCosts, dto.OtherCosts);
            setter.UpdateField(e => e.DeliveryCostWithoutVAT, dto.DeliveryCostWithoutVAT, new DeliveryCostWithoutVATHandler());
            setter.UpdateField(e => e.ReturnCostWithoutVAT, dto.ReturnCostWithoutVAT, new ReturnCostWithoutVATHandler());
            setter.UpdateField(e => e.InvoiceAmountWithoutVAT, dto.InvoiceAmountWithoutVAT);
            setter.UpdateField(e => e.AdditionalCostsWithoutVAT, dto.AdditionalCostsWithoutVAT, new AdditionalCostsWithoutVATHandler());
            setter.UpdateField(e => e.AdditionalCostsComments, dto.AdditionalCostsComments);
            setter.UpdateField(e => e.TrucksDowntime, dto.TrucksDowntime, new TrucksDowntimeHandler());
            setter.UpdateField(e => e.ReturnRate, dto.ReturnRate);
            setter.UpdateField(e => e.AdditionalPointRate, dto.AdditionalPointRate);
            setter.UpdateField(e => e.DowntimeRate, dto.DowntimeRate);
            setter.UpdateField(e => e.BlankArrivalRate, dto.BlankArrivalRate);
            setter.UpdateField(e => e.BlankArrival, dto.BlankArrival ?? false);
            setter.UpdateField(e => e.Waybill, dto.Waybill ?? false);
            setter.UpdateField(e => e.WaybillTorg12, dto.WaybillTorg12 ?? false);
            setter.UpdateField(e => e.TransportWaybill, dto.TransportWaybill ?? false);
            setter.UpdateField(e => e.Invoice, dto.Invoice ?? false);
            setter.UpdateField(e => e.DocumentsReturnDate, ParseDateTime(dto.DocumentsReturnDate));
            setter.UpdateField(e => e.ActualDocumentsReturnDate, ParseDateTime(dto.ActualDocumentsReturnDate));
            setter.UpdateField(e => e.InvoiceNumber, dto.InvoiceNumber);
            if (!string.IsNullOrEmpty(dto.Status))
                setter.UpdateField(e => e.Status,  MapFromStateDto<ShippingState>(dto.Status));
            setter.UpdateField(e => e.CostsConfirmedByShipper, dto.CostsConfirmedByShipper ?? false);
            setter.UpdateField(e => e.CostsConfirmedByCarrier, dto.CostsConfirmedByCarrier ?? false);
            /*end of map dto to entity fields*/

            setter.ApplyAfterActions();

            string errors = setter.ValidationErrors;
            return new ValidateResult(errors, entity.Id.ToString());
        }

        public override ValidateResult MapFromFormDtoToEntity(Shipping entity, ShippingDto dto)
        {
            return MapFromDtoToEntity(entity, dto);
        }

        public override ShippingDto MapFromEntityToDto(Shipping entity)
        {
            return _mapper.Map<ShippingDto>(entity);
        }

        public override ShippingDto MapFromEntityToFormDto(Shipping entity)
        {
            return MapFromEntityToDto(entity);
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Shipping, ShippingDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.Status, e => e.MapFrom((s, t) => s.Status?.ToString()?.ToLowerfirstLetter()))
                    .ForMember(t => t.DeliveryType, e => e.MapFrom((s, t) => s.DeliveryType?.ToString()))
                    .ForMember(t => t.CarrierId, e => e.MapFrom((s, t) => s.CarrierId?.ToString()))
                    .ForMember(t => t.VehicleTypeId, e => e.MapFrom((s, t) => s.VehicleTypeId?.ToString()))
                    .ForMember(t => t.TarifficationType, e => e.MapFrom((s, t) => s.TarifficationType?.ToString()))
                    .ForMember(t => t.LoadingArrivalTime, e => e.MapFrom((s, t) => s.LoadingArrivalTime?.ToString("dd.MM.yyyy HH:mm")))
                    .ForMember(t => t.LoadingDepartureTime, e => e.MapFrom((s, t) => s.LoadingDepartureTime?.ToString("dd.MM.yyyy HH:mm")))
                    .ForMember(t => t.DocumentsReturnDate, e => e.MapFrom((s, t) => s.DocumentsReturnDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ActualDocumentsReturnDate, e => e.MapFrom((s, t) => s.ActualDocumentsReturnDate?.ToString("dd.MM.yyyy")));
            });
            return result;
        }

        private readonly IMapper _mapper;
    }
}
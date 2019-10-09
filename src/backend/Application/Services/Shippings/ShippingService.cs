using System;
using System.Collections.Generic;
using System.Linq;
using Application.BusinessModels.Shippings.Actions;
using Application.BusinessModels.Shippings.Handlers;
using Application.Shared;
using AutoMapper;
using DAL;
using DAL.Queries;
using Domain;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Shippings;
using Domain.Services.UserIdProvider;
using Domain.Shared;
using Domain.Shared.FormFilters;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Shippings
{
    public class ShippingsService : GridWithDocumentsBase<Shipping, ShippingDto, ShippingFormDto, FilterForm<SearchFilter>>, IShippingsService
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

        public override ValidateResult MapFromFormDtoToEntity(Shipping entity, ShippingFormDto dto)
        {
            var result = MapFromDtoToEntity(entity, dto);
            if (!result.IsError)
            {
                result = SaveRoutePoints(entity, dto);
            }
            return result;
        }

        public override ShippingDto MapFromEntityToDto(Shipping entity)
        {
            return _mapper.Map<ShippingDto>(entity);
        }

        public override ShippingFormDto MapFromEntityToFormDto(Shipping entity)
        {
            ShippingDto dto = MapFromEntityToDto(entity);
            ShippingFormDto formDto = _mapper.Map<ShippingFormDto>(dto);
            formDto.RoutePoints = GetRoutePoints(entity);
            return formDto;
        }

        private ValidateResult SaveRoutePoints(Shipping entity, ShippingFormDto dto)
        {
            if (dto.RoutePoints != null)
            {
                var orders = db.Orders.Where(o => o.ShippingId == entity.Id).ToList();
                var ordersDict = orders.ToDictionary(o => o.Id.ToString());
                
                foreach (RoutePointDto pointDto in dto.RoutePoints)
                {
                    if (pointDto.OrderIds == null)
                    {
                        continue;
                    }

                    foreach (string orderId in pointDto.OrderIds)
                    {
                        Order order;
                        if (ordersDict.TryGetValue(orderId, out order))
                        {
                            var setter = new FieldSetter<Order>(order);

                            if (pointDto.IsLoading)
                            {
                                setter.UpdateField(o => o.ShippingDate, ParseDateTime(pointDto.PlannedDate));
                                setter.UpdateField(o => o.LoadingArrivalTime, ParseDateTime(pointDto.ArrivalTime));
                                setter.UpdateField(o => o.LoadingDepartureTime, ParseDateTime(pointDto.DepartureTime));
                                if (!string.IsNullOrEmpty(pointDto.VehicleStatus))
                                    setter.UpdateField(e => e.ShippingStatus, MapFromStateDto<VehicleState>(pointDto.VehicleStatus));
                            }
                            else
                            {
                                setter.UpdateField(o => o.DeliveryDate, ParseDateTime(pointDto.PlannedDate));
                                setter.UpdateField(o => o.UnloadingArrivalTime, ParseDateTime(pointDto.ArrivalTime));
                                setter.UpdateField(o => o.UnloadingDepartureTime, ParseDateTime(pointDto.DepartureTime));
                                if (!string.IsNullOrEmpty(pointDto.VehicleStatus))
                                    setter.UpdateField(e => e.DeliveryStatus, MapFromStateDto<VehicleState>(pointDto.VehicleStatus));
                            }

                            setter.ApplyAfterActions();
                        }
                    }
                }
            }

            return new ValidateResult(null, entity.Id.ToString());
        }

        private List<RoutePointDto> GetRoutePoints(Shipping entity)
        {
            var points = new Dictionary<string, RoutePointDto>();
            var orders = db.Orders.Where(o => o.ShippingId == entity.Id).ToList();
            foreach (Order order in orders)
            {
                if (order.ShippingWarehouseId.HasValue)
                {
                    RoutePointDto point;
                    string key = $"L-{order.ShippingWarehouseId.ToString()}";
                    if (!points.TryGetValue(key, out point))
                    {
                        point = new RoutePointDto
                        {
                            WarehouseName = db.Warehouses.GetById(order.ShippingWarehouseId.Value)?.WarehouseName,
                            Address = order.ShippingAddress,
                            PlannedDate = order.ShippingDate?.ToString("dd.MM.yyyy HH:mm"),
                            ArrivalTime = order.LoadingArrivalTime?.ToString("dd.MM.yyyy HH:mm"),
                            DepartureTime = order.LoadingDepartureTime?.ToString("dd.MM.yyyy HH:mm"),
                            VehicleStatus = order.ShippingStatus.ToString().ToLowerfirstLetter(),
                            IsLoading = true,
                            OrderIds = new List<string>()
                        };
                        points[key] = point;
                    }
                    point.OrderIds.Add(order.Id.ToString());
                }

                if (order.DeliveryWarehouseId.HasValue)
                {
                    RoutePointDto point;
                    string key = $"U-{order.DeliveryWarehouseId.ToString()}";
                    if (!points.TryGetValue(key, out point))
                    {
                        point = new RoutePointDto
                        {
                            WarehouseName = order.ClientName,
                            Address = order.DeliveryAddress,
                            PlannedDate = order.DeliveryDate?.ToString("dd.MM.yyyy HH:mm"),
                            ArrivalTime = order.UnloadingArrivalTime?.ToString("dd.MM.yyyy HH:mm"),
                            DepartureTime = order.UnloadingDepartureTime?.ToString("dd.MM.yyyy HH:mm"),
                            VehicleStatus = order.DeliveryStatus.ToString().ToLowerfirstLetter(),
                            IsLoading = false,
                            OrderIds = new List<string>()
                        };
                        points[key] = point;
                    }
                    point.OrderIds.Add(order.Id.ToString());
                }
            }

            var pointsList = points.Values.OrderBy(p => p.PlannedDate)
                                          .ThenBy(p => p.IsLoading ? 0 : 1)
                                          .ThenBy(p => p.VehicleStatus)
                                          .ThenBy(p => p.WarehouseName)
                                          .ToList();
            return pointsList;
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ShippingDto, ShippingFormDto>();

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

        public override IQueryable<Shipping> ApplySearchForm(IQueryable<Shipping> query, FilterForm<SearchFilter> searchForm)
        {
            return query;
        }
    }
}
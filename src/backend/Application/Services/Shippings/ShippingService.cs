using Application.BusinessModels.Shared.Actions;
using Application.BusinessModels.Shared.BulkUpdates;
using Application.BusinessModels.Shippings.Handlers;
using Application.Extensions;
using Application.Shared;
using AutoMapper;
using DAL;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Services.Shippings;
using Domain.Services.UserProvider;
using Domain.Shared;
using Domain.Shared.FormFilters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Application.Services.Shippings
{
    public class ShippingsService : GridService<Shipping, ShippingDto, ShippingFormDto, ShippingSummaryDto, FilterFormDto<ShippingFilterDto>>, IShippingsService
    {
        private readonly IHistoryService _historyService;

        public ShippingsService(
            IHistoryService historyService,
            ICommonDataService dataService,
            IUserProvider userIdProvider,
            IEnumerable<IAppAction<Shipping>> singleActions,
            IEnumerable<IGroupAppAction<Shipping>> groupActions,
            IEnumerable<IBulkUpdate<Shipping>> bulkUpdates)
            : base(dataService, userIdProvider, singleActions, groupActions, bulkUpdates)
        {
            _mapper = ConfigureMapper().CreateMapper();
            this._historyService = historyService;
        }

        public override LookUpDto MapFromEntityToLookupDto(Shipping entity)
        {
            return new LookUpDto
            {
                Value = entity.Id.ToString(),
                Name = entity.ShippingNumber
            };
        }

        public override ShippingSummaryDto GetSummary(IEnumerable<Guid> ids)
        {
            return new ShippingSummaryDto();
        }

        public override ValidateResult MapFromDtoToEntity(Shipping entity, ShippingDto dto)
        {
            var setter = new FieldSetter<Shipping>(entity, _historyService);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id), ignoreChanges: true);
            setter.UpdateField(e => e.ShippingNumber, dto.ShippingNumber);
            setter.UpdateField(e => e.DeliveryType, string.IsNullOrEmpty(dto.DeliveryType) ? (DeliveryType?)null : MapFromStateDto<DeliveryType>(dto.DeliveryType));
            setter.UpdateField(e => e.TemperatureMin, dto.TemperatureMin);
            setter.UpdateField(e => e.TemperatureMax, dto.TemperatureMax);
            setter.UpdateField(e => e.TarifficationType, string.IsNullOrEmpty(dto.TarifficationType) ? (TarifficationType?)null : MapFromStateDto<TarifficationType>(dto.TarifficationType));
            setter.UpdateField(e => e.CarrierId, string.IsNullOrEmpty(dto.CarrierId) ? (Guid?)null : Guid.Parse(dto.CarrierId), nameLoader: GetCarrierNameById);
            setter.UpdateField(e => e.VehicleTypeId, string.IsNullOrEmpty(dto.VehicleTypeId) ? (Guid?)null : Guid.Parse(dto.VehicleTypeId), nameLoader: GetVehicleTypeNameById);
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
            setter.UpdateField(e => e.DeliveryCostWithoutVAT, dto.DeliveryCostWithoutVAT, new DeliveryCostWithoutVATHandler(_historyService));
            setter.UpdateField(e => e.ReturnCostWithoutVAT, dto.ReturnCostWithoutVAT, new ReturnCostWithoutVATHandler(_historyService));
            setter.UpdateField(e => e.InvoiceAmountWithoutVAT, dto.InvoiceAmountWithoutVAT);
            setter.UpdateField(e => e.AdditionalCostsWithoutVAT, dto.AdditionalCostsWithoutVAT, new AdditionalCostsWithoutVATHandler(_historyService));
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
            setter.UpdateField(e => e.CostsConfirmedByShipper, dto.CostsConfirmedByShipper ?? false);
            setter.UpdateField(e => e.CostsConfirmedByCarrier, dto.CostsConfirmedByCarrier ?? false);
            /*end of map dto to entity fields*/

            setter.ApplyAfterActions();
            setter.SaveHistoryLog();

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
            if (entity == null)
            {
                return null;
            }
            return _mapper.Map<ShippingDto>(entity);
        }

        public override ShippingFormDto MapFromEntityToFormDto(Shipping entity)
        {
            if (entity == null)
            {
                return null;
            }

            ShippingDto dto = MapFromEntityToDto(entity);
            ShippingFormDto formDto = _mapper.Map<ShippingFormDto>(dto);

            var orders = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == entity.Id).ToList();
            formDto.Orders = GetShippingOrders(orders);
            formDto.RoutePoints = GetRoutePoints(entity, orders);

            return formDto;
        }

        private string GetCarrierNameById(Guid? id)
        {
            return id == null ? null : _dataService.GetById<TransportCompany>(id.Value)?.Title;
        }

        private string GetVehicleTypeNameById(Guid? id)
        {
            return id == null ? null : _dataService.GetById<VehicleType>(id.Value)?.Name;
        }

        private ValidateResult SaveRoutePoints(Shipping entity, ShippingFormDto dto)
        {
            if (dto.RoutePoints != null)
            {
                var orders = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == entity.Id).ToList();
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
                            var setter = new FieldSetter<Order>(order, _historyService);

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
                                setter.UpdateField(o => o.TrucksDowntime, pointDto.TrucksDowntime);
                                if (!string.IsNullOrEmpty(pointDto.VehicleStatus))
                                    setter.UpdateField(e => e.DeliveryStatus, MapFromStateDto<VehicleState>(pointDto.VehicleStatus));
                            }

                            setter.ApplyAfterActions();
                            setter.SaveHistoryLog();
                        }
                    }
                }
            }

            return new ValidateResult(null, entity.Id.ToString());
        }

        private List<ShippingOrderDto> GetShippingOrders(List<Order> orders)
        {
            List<ShippingOrderDto> result = new List<ShippingOrderDto>();
            foreach(Order order in orders.OrderBy(o => o.OrderNumber))
            {
                ShippingOrderDto dto = new ShippingOrderDto
                {
                    Id = order.Id.ToString(),
                    OrderNumber = order.OrderNumber,
                    Status = order.Status.ToString().ToLowerfirstLetter()
                };
                result.Add(dto);
            }
            return result;
        }

        private List<RoutePointDto> GetRoutePoints(Shipping entity, List<Order> orders)
        {
            var points = new Dictionary<string, RoutePointDto>();
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
                            WarehouseName = _dataService.GetById<Warehouse>(order.ShippingWarehouseId.Value)?.WarehouseName,
                            Address = order.ShippingAddress,
                            PlannedDate = order.ShippingDate?.ToString("dd.MM.yyyy HH:mm"),
                            ArrivalTime = order.LoadingArrivalTime?.ToString("dd.MM.yyyy HH:mm"),
                            DepartureTime = order.LoadingDepartureTime?.ToString("dd.MM.yyyy HH:mm"),
                            VehicleStatus = order.ShippingStatus.ToString().ToLowerfirstLetter(),
                            TrucksDowntime = null,
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
                            TrucksDowntime = order.TrucksDowntime,
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
                    .ForMember(t => t.DeliveryType, e => e.MapFrom((s, t) => s.DeliveryType?.ToString()?.ToLowerfirstLetter()))
                    .ForMember(t => t.CarrierId, e => e.MapFrom((s, t) => s.CarrierId?.ToString()))
                    .ForMember(t => t.VehicleTypeId, e => e.MapFrom((s, t) => s.VehicleTypeId?.ToString()))
                    .ForMember(t => t.TarifficationType, e => e.MapFrom((s, t) => s.TarifficationType?.ToString()?.ToLowerfirstLetter()))
                    .ForMember(t => t.LoadingArrivalTime, e => e.MapFrom((s, t) => s.LoadingArrivalTime?.ToString("dd.MM.yyyy HH:mm")))
                    .ForMember(t => t.LoadingDepartureTime, e => e.MapFrom((s, t) => s.LoadingDepartureTime?.ToString("dd.MM.yyyy HH:mm")))
                    .ForMember(t => t.DocumentsReturnDate, e => e.MapFrom((s, t) => s.DocumentsReturnDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ActualDocumentsReturnDate, e => e.MapFrom((s, t) => s.ActualDocumentsReturnDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ShippingCreationDate, e => e.MapFrom((s, t) => s.ShippingCreationDate?.ToString("dd.MM.yyyy HH:mm")));
            });
            return result;
        }

        private readonly IMapper _mapper;

        public override IQueryable<Shipping> ApplySearchForm(IQueryable<Shipping> query, FilterFormDto<ShippingFilterDto> searchForm)
        {
            query = query
                .ApplyStringFilter(i => i.ShippingNumber, searchForm.Filter.ShippingNumber)
                .ApplyOptionsFilter(i => i.CarrierId.Value.ToString(), searchForm.Filter.CarrierId)
                .ApplyEnumFilter(i => i.TarifficationType, searchForm.Filter.TarifficationType)
                .ApplyEnumFilter(i => i.DeliveryType, searchForm.Filter.DeliveryType)
                .ApplyDateRangeFilter(i => i.ShippingCreationDate.Value, searchForm.Filter.ShippingCreationDate)
                .ApplyNumericFilter(i => i.TemperatureMin.Value, searchForm.Filter.TemperatureMin)
                .ApplyNumericFilter(i => i.TemperatureMax.Value, searchForm.Filter.TemperatureMax)
                .ApplyOptionsFilter(i => i.VehicleTypeId, searchForm.Filter.VehicleTypeId, i => new Guid(i))
                .ApplyNumericFilter(i => i.PalletsCount.Value, searchForm.Filter.PalletsCount)
                .ApplyNumericFilter(i => i.ActualPalletsCount.Value, searchForm.Filter.ActualPalletsCount)
                .ApplyNumericFilter(i => i.ConfirmedPalletsCount.Value, searchForm.Filter.ConfirmedPalletsCount)
                .ApplyNumericFilter(i => i.WeightKg.Value, searchForm.Filter.WeightKg)
                .ApplyNumericFilter(i => i.ActualWeightKg.Value, searchForm.Filter.ActualWeightKg)
                .ApplyDateRangeFilter(i => i.LoadingArrivalTime.Value, searchForm.Filter.LoadingArrivalTime)
                .ApplyDateRangeFilter(i => i.LoadingDepartureTime.Value, searchForm.Filter.LoadingDepartureTime)
                .ApplyStringFilter(i => i.DeliveryInvoiceNumber,searchForm.Filter.DeliveryInvoiceNumber)
                .ApplyStringFilter(i => i.DeviationReasonsComments, searchForm.Filter.DeviationReasonsComments)
                .ApplyNumericFilter(i => i.TotalDeliveryCost.Value, searchForm.Filter.TotalDeliveryCost)
                .ApplyNumericFilter(i => i.OtherCosts.Value, searchForm.Filter.OtherCosts)
                .ApplyNumericFilter(i => i.DeliveryCostWithoutVAT.Value, searchForm.Filter.DeliveryCostWithoutVAT)
                .ApplyNumericFilter(i => i.ReturnCostWithoutVAT.Value, searchForm.Filter.ReturnCostWithoutVAT)
                .ApplyNumericFilter(i => i.InvoiceAmountWithoutVAT.Value, searchForm.Filter.InvoiceAmountWithoutVAT)
                .ApplyNumericFilter(i => i.AdditionalCostsWithoutVAT.Value, searchForm.Filter.AdditionalCostsWithoutVAT)
                .ApplyStringFilter(i => i.AdditionalCostsComments, searchForm.Filter.AdditionalCostsComments)
                .ApplyNumericFilter(i => i.TrucksDowntime.Value, searchForm.Filter.TrucksDowntime)
                .ApplyNumericFilter(i => i.ReturnRate.Value, searchForm.Filter.ReturnRate)
                .ApplyNumericFilter(i => i.AdditionalPointRate.Value, searchForm.Filter.AdditionalPointRate)
                .ApplyNumericFilter(i => i.DowntimeRate.Value, searchForm.Filter.DowntimeRate)
                .ApplyNumericFilter(i => i.BlankArrivalRate.Value, searchForm.Filter.BlankArrivalRate)
                .ApplyBooleanFilter(i => i.BlankArrival, searchForm.Filter.BlankArrival)
                .ApplyBooleanFilter(i => i.Waybill, searchForm.Filter.Waybill)
                .ApplyBooleanFilter(i => i.WaybillTorg12, searchForm.Filter.WaybillTorg12)
                .ApplyBooleanFilter(i => i.TransportWaybill, searchForm.Filter.TransportWaybill)
                .ApplyBooleanFilter(i => i.Invoice, searchForm.Filter.Invoice)
                .ApplyDateRangeFilter(i => i.DocumentsReturnDate.Value, searchForm.Filter.DocumentsReturnDate)
                .ApplyDateRangeFilter(i => i.ActualDocumentsReturnDate.Value, searchForm.Filter.ActualDocumentsReturnDate)
                .ApplyStringFilter(i => i.InvoiceNumber, searchForm.Filter.InvoiceNumber)
                .ApplyBooleanFilter(i => i.CostsConfirmedByShipper, searchForm.Filter.CostsConfirmedByShipper)
                .ApplyBooleanFilter(i => i.CostsConfirmedByCarrier, searchForm.Filter.CostsConfirmedByCarrier);

            // Apply Search
            query = this.ApplySearch(query, searchForm);

            return query.OrderBy(searchForm.Sort.Name, searchForm.Sort.Desc)
                .DefaultOrderBy(i => i.ShippingCreationDate, !string.IsNullOrEmpty(searchForm.Sort?.Name));
        }

        private IQueryable<Shipping> ApplySearch(IQueryable<Shipping> query, FilterFormDto<ShippingFilterDto> searchForm)
        {
            var search = searchForm.Filter.Search;

            if (string.IsNullOrEmpty(search)) return query;

            var isInt = int.TryParse(search, out int searchInt);
            var isDecimal = decimal.TryParse(search, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal searchDecimal);
            decimal precision = 0.01M;

            var searchDateFormat = "dd.MM.yyyy HH:mm";

            var tarrificationTypes = Enum.GetNames(typeof(TarifficationType))
                .Where(i => i.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                .Select(i => (TarifficationType?)MapFromStateDto<TarifficationType>(i));

            var deliveryTypes = Enum.GetNames(typeof(DeliveryType))
            .Where(i => i.Contains(search, StringComparison.InvariantCultureIgnoreCase))
            .Select(i => (DeliveryType?)MapFromStateDto<DeliveryType>(i));

            var transportCompanies = this._dataService.GetDbSet<TransportCompany>()
                .Where(i => i.Title.Contains(search))
                .Select(i => i.Id);

            var vehicleTypes = this._dataService.GetDbSet<VehicleType>()
                .Where(i => i.Name.Contains(search))
                .Select(i => i.Id);

            return query.Where(i => 
               !string.IsNullOrEmpty(i.ShippingNumber) && i.ShippingNumber.Contains(search)
            || !string.IsNullOrEmpty(i.DeliveryInvoiceNumber) && i.DeliveryInvoiceNumber.Contains(search)
            || !string.IsNullOrEmpty(i.DeviationReasonsComments) && i.DeviationReasonsComments.Contains(search)
            || !string.IsNullOrEmpty(i.AdditionalCostsComments) && i.AdditionalCostsComments.Contains(search)
            || !string.IsNullOrEmpty(i.InvoiceNumber) && i.InvoiceNumber.Contains(search)
            || !string.IsNullOrEmpty(i.InvoiceNumber) && i.InvoiceNumber.Contains(search)
            || isInt && i.TemperatureMin == searchInt
            || isInt && i.TemperatureMax == searchInt
            || isInt && i.PalletsCount == searchInt
            || isInt && i.ActualPalletsCount == searchInt
            || isInt && i.ConfirmedPalletsCount == searchInt
            || isDecimal && i.WeightKg >= searchDecimal - precision && i.WeightKg <= searchDecimal + precision
            || isDecimal && i.ActualWeightKg >= searchDecimal - precision && i.ActualWeightKg <= searchDecimal + precision
            || isDecimal && i.TotalDeliveryCost >= searchDecimal - precision && i.TotalDeliveryCost <= searchDecimal + precision
            || isDecimal && i.OtherCosts >= searchDecimal - precision && i.OtherCosts <= searchDecimal + precision
            || isDecimal && i.DeliveryCostWithoutVAT >= searchDecimal - precision && i.DeliveryCostWithoutVAT <= searchDecimal + precision
            || isDecimal && i.ReturnCostWithoutVAT >= searchDecimal - precision && i.ReturnCostWithoutVAT <= searchDecimal + precision
            || isDecimal && i.InvoiceAmountWithoutVAT >= searchDecimal - precision && i.InvoiceAmountWithoutVAT <= searchDecimal + precision
            || isDecimal && i.AdditionalCostsWithoutVAT >= searchDecimal - precision && i.AdditionalCostsWithoutVAT <= searchDecimal + precision
            || isDecimal && i.TrucksDowntime >= searchDecimal - precision && i.TrucksDowntime <= searchDecimal + precision
            || isDecimal && i.ReturnRate >= searchDecimal - precision && i.ReturnRate <= searchDecimal + precision
            || isDecimal && i.AdditionalPointRate >= searchDecimal - precision && i.AdditionalPointRate <= searchDecimal + precision
            || isDecimal && i.DowntimeRate >= searchDecimal - precision && i.DowntimeRate <= searchDecimal + precision
            || isDecimal && i.BlankArrivalRate >= searchDecimal - precision && i.BlankArrivalRate <= searchDecimal + precision
            || i.ShippingCreationDate.HasValue && i.ShippingCreationDate.Value.ToString(searchDateFormat).Contains(search)
            || i.LoadingArrivalTime.HasValue && i.LoadingArrivalTime.Value.ToString(searchDateFormat).Contains(search)
            || i.LoadingDepartureTime.HasValue && i.LoadingDepartureTime.Value.ToString(searchDateFormat).Contains(search)
            || i.DocumentsReturnDate.HasValue && i.DocumentsReturnDate.Value.ToString(searchDateFormat).Contains(search)
            || i.ActualDocumentsReturnDate.HasValue && i.ActualDocumentsReturnDate.Value.ToString(searchDateFormat).Contains(search)
            || tarrificationTypes.Contains(i.TarifficationType)
            || deliveryTypes.Contains(i.DeliveryType)
            || vehicleTypes.Any(v => v == i.VehicleTypeId)
            || transportCompanies.Any(t => t == i.CarrierId)
            );
        }
    }
}

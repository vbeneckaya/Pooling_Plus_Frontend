using Application.BusinessModels.Shared.Actions;
using Application.BusinessModels.Shippings.Handlers;
using Application.Extensions;
using Application.Shared;
using AutoMapper;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.FieldProperties;
using Domain.Services.History;
using Domain.Services.Shippings;
using Domain.Services.UserProvider;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Application.Services.Shippings
{
    public class ShippingsService : GridService<Shipping, ShippingDto, ShippingFormDto, ShippingSummaryDto, ShippingFilterDto>, IShippingsService
    {
        private readonly IHistoryService _historyService;
        private readonly IFieldPropertiesService _fieldPropertiesService;

        public ShippingsService(
            IHistoryService historyService,
            ICommonDataService dataService,
            IUserProvider userIdProvider,
            IFieldDispatcherService fieldDispatcherService,
            IFieldPropertiesService fieldPropertiesService,
            IEnumerable<IAppAction<Shipping>> singleActions,
            IEnumerable<IGroupAppAction<Shipping>> groupActions)
            : base(dataService, userIdProvider, fieldDispatcherService, fieldPropertiesService, singleActions, groupActions)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
            _fieldPropertiesService = fieldPropertiesService;
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

        public IEnumerable<LookUpDto> FindByNumber(NumberSearchFormDto dto)
        {
            var dbSet = _dataService.GetDbSet<Shipping>();
            List<Shipping> entities;
            if (dto.IsPartial)
            {
                entities = dbSet.Where(x => x.ShippingNumber.Contains(dto.Number, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            else
            {
                entities = dbSet.Where(x => x.ShippingNumber == dto.Number).ToList();
            }
            var result = entities.Select(MapFromEntityToLookupDto);
            return result;
        }

        public override IEnumerable<EntityStatusDto> LoadStatusData(IEnumerable<Guid> ids)
        {
            var result = _dataService.GetDbSet<Shipping>()
                            .Where(x => ids.Contains(x.Id))
                            .Select(x => new EntityStatusDto { Id = x.Id.ToString(), Status = x.Status.ToString() })
                            .ToList();
            return result;
        }

        public override ValidateResult MapFromDtoToEntity(Shipping entity, ShippingDto dto)
        {
            bool isNew = string.IsNullOrEmpty(dto.Id);

            IEnumerable<string> readOnlyFields = null;
            if (!isNew)
            {
                var userId = _userIdProvider.GetCurrentUserId();
                if (userId != null)
                {
                    string stateName = entity.Status?.ToString()?.ToLowerFirstLetter();
                    readOnlyFields = _fieldPropertiesService.GetReadOnlyFields(FieldPropertiesForEntityType.Shippings, stateName, null, null, userId);
                }
            }

            var setter = new FieldSetter<Shipping>(entity, _historyService, readOnlyFields);

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
                    Status = order.Status.ToString().ToLowerFirstLetter()
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
                            VehicleStatus = order.ShippingStatus.ToString().ToLowerFirstLetter(),
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
                            VehicleStatus = order.DeliveryStatus.ToString().ToLowerFirstLetter(),
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
                    .ForMember(t => t.Status, e => e.MapFrom((s, t) => s.Status?.ToString()?.ToLowerFirstLetter()))
                    .ForMember(t => t.DeliveryType, e => e.MapFrom((s, t) => s.DeliveryType?.ToString()?.ToLowerFirstLetter()))
                    .ForMember(t => t.CarrierId, e => e.MapFrom((s, t) => s.CarrierId?.ToString()))
                    .ForMember(t => t.VehicleTypeId, e => e.MapFrom((s, t) => s.VehicleTypeId?.ToString()))
                    .ForMember(t => t.TarifficationType, e => e.MapFrom((s, t) => s.TarifficationType?.ToString()?.ToLowerFirstLetter()))
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
            List<object> parameters = new List<object>();
            string where = string.Empty;

            where = where
                         .WhereAnd(searchForm.Filter.ActualDocumentsReturnDate.ApplyDateRangeFilter<Shipping>(i => i.ActualDocumentsReturnDate, ref parameters))
                         .WhereAnd(searchForm.Filter.ActualPalletsCount.ApplyNumericFilter<Shipping>(i => i.ActualPalletsCount, ref parameters))
                         .WhereAnd(searchForm.Filter.ActualWeightKg.ApplyNumericFilter<Shipping>(i => i.ActualWeightKg, ref parameters))
                         .WhereAnd(searchForm.Filter.AdditionalCostsComments.ApplyStringFilter<Shipping>(i => i.AdditionalCostsComments, ref parameters))
                         .WhereAnd(searchForm.Filter.AdditionalCostsWithoutVAT.ApplyNumericFilter<Shipping>(i => i.AdditionalCostsWithoutVAT, ref parameters))
                         .WhereAnd(searchForm.Filter.AdditionalPointRate.ApplyNumericFilter<Shipping>(i => i.AdditionalPointRate, ref parameters))
                         .WhereAnd(searchForm.Filter.BlankArrival.ApplyBooleanFilter<Shipping>(i => i.BlankArrival, ref parameters))
                         .WhereAnd(searchForm.Filter.BlankArrivalRate.ApplyNumericFilter<Shipping>(i => i.BlankArrivalRate, ref parameters))
                         .WhereAnd(searchForm.Filter.CarrierId.ApplyOptionsFilter<Shipping, Guid?>(i => i.CarrierId, ref parameters, i => new Guid(i)))
                         .WhereAnd(searchForm.Filter.ConfirmedPalletsCount.ApplyNumericFilter<Shipping>(i => i.ConfirmedPalletsCount, ref parameters))
                         .WhereAnd(searchForm.Filter.CostsConfirmedByCarrier.ApplyBooleanFilter<Shipping>(i => i.CostsConfirmedByCarrier, ref parameters))
                         .WhereAnd(searchForm.Filter.CostsConfirmedByShipper.ApplyBooleanFilter<Shipping>(i => i.CostsConfirmedByShipper, ref parameters))
                         .WhereAnd(searchForm.Filter.DeliveryInvoiceNumber.ApplyStringFilter<Shipping>(i => i.DeliveryInvoiceNumber, ref parameters))
                         .WhereAnd(searchForm.Filter.DeliveryType.ApplyEnumFilter<Shipping, DeliveryType>(i => i.DeliveryType, ref parameters))
                         .WhereAnd(searchForm.Filter.DeliveryCostWithoutVAT.ApplyNumericFilter<Shipping>(i => i.DeliveryCostWithoutVAT, ref parameters))
                         .WhereAnd(searchForm.Filter.DeviationReasonsComments.ApplyStringFilter<Shipping>(i => i.DeviationReasonsComments, ref parameters))
                         .WhereAnd(searchForm.Filter.DocumentsReturnDate.ApplyDateRangeFilter<Shipping>(i => i.DocumentsReturnDate, ref parameters))
                         .WhereAnd(searchForm.Filter.DowntimeRate.ApplyNumericFilter<Shipping>(i => i.DowntimeRate, ref parameters))
                         .WhereAnd(searchForm.Filter.Invoice.ApplyBooleanFilter<Shipping>(i => i.Invoice, ref parameters))
                         .WhereAnd(searchForm.Filter.InvoiceAmountWithoutVAT.ApplyNumericFilter<Shipping>(i => i.InvoiceAmountWithoutVAT, ref parameters))
                         .WhereAnd(searchForm.Filter.InvoiceNumber.ApplyStringFilter<Shipping>(i => i.InvoiceNumber, ref parameters))
                         .WhereAnd(searchForm.Filter.LoadingArrivalTime.ApplyDateRangeFilter<Shipping>(i => i.LoadingArrivalTime, ref parameters))
                         .WhereAnd(searchForm.Filter.LoadingDepartureTime.ApplyDateRangeFilter<Shipping>(i => i.LoadingDepartureTime, ref parameters))
                         .WhereAnd(searchForm.Filter.OtherCosts.ApplyNumericFilter<Shipping>(i => i.OtherCosts, ref parameters))
                         .WhereAnd(searchForm.Filter.PalletsCount.ApplyNumericFilter<Shipping>(i => i.PalletsCount, ref parameters))
                         .WhereAnd(searchForm.Filter.ReturnCostWithoutVAT.ApplyNumericFilter<Shipping>(i => i.ReturnCostWithoutVAT, ref parameters))
                         .WhereAnd(searchForm.Filter.ReturnRate.ApplyNumericFilter<Shipping>(i => i.ReturnRate, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingCreationDate.ApplyDateRangeFilter<Shipping>(i => i.ShippingCreationDate, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingNumber.ApplyStringFilter<Shipping>(i => i.ShippingNumber, ref parameters))
                         .WhereAnd(searchForm.Filter.Status.ApplyEnumFilter<Shipping, ShippingState>(i => i.Status, ref parameters))
                         .WhereAnd(searchForm.Filter.TarifficationType.ApplyEnumFilter<Shipping, TarifficationType>(i => i.TarifficationType, ref parameters))
                         .WhereAnd(searchForm.Filter.TemperatureMax.ApplyNumericFilter<Shipping>(i => i.TemperatureMax, ref parameters))
                         .WhereAnd(searchForm.Filter.TemperatureMin.ApplyNumericFilter<Shipping>(i => i.TemperatureMin, ref parameters))
                         .WhereAnd(searchForm.Filter.TotalDeliveryCost.ApplyNumericFilter<Shipping>(i => i.TotalDeliveryCost, ref parameters))
                         .WhereAnd(searchForm.Filter.TransportWaybill.ApplyBooleanFilter<Shipping>(i => i.TransportWaybill, ref parameters))
                         .WhereAnd(searchForm.Filter.TrucksDowntime.ApplyNumericFilter<Shipping>(i => i.TrucksDowntime, ref parameters))
                         .WhereAnd(searchForm.Filter.VehicleTypeId.ApplyOptionsFilter<Shipping, Guid?>(i => i.VehicleTypeId, ref parameters, i => new Guid(i)))
                         .WhereAnd(searchForm.Filter.Waybill.ApplyBooleanFilter<Shipping>(i => i.Waybill, ref parameters))
                         .WhereAnd(searchForm.Filter.WaybillTorg12.ApplyBooleanFilter<Shipping>(i => i.WaybillTorg12, ref parameters))
                         .WhereAnd(searchForm.Filter.WeightKg.ApplyNumericFilter<Shipping>(i => i.WeightKg, ref parameters));

            string sql = $@"SELECT * FROM ""Shippings"" {where}";
            query = query.FromSql(sql, parameters.ToArray());

            // Apply Search
            query = this.ApplySearch(query, searchForm);

            return query.OrderBy(searchForm.Sort?.Name, searchForm.Sort?.Desc)
                .DefaultOrderBy(i => i.ShippingCreationDate, !string.IsNullOrEmpty(searchForm.Sort?.Name))
                .DefaultOrderBy(i => i.Id, true);
        }

        private IQueryable<Shipping> ApplySearch(IQueryable<Shipping> query, FilterFormDto<ShippingFilterDto> searchForm)
        {
            var search = searchForm.Filter.Search;
            var columns = searchForm.Filter.Columns;

            if (string.IsNullOrEmpty(search)) return query;

            var isInt = int.TryParse(search, out int searchInt);
            var isDecimal = decimal.TryParse(search.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal searchDecimal);
            decimal precision = 0.01M;

            var searchDateFormat = "dd.MM.yyyy HH:mm";

            //TarifficationType search

            var tarifficationTypeNames = Enum.GetNames(typeof(TarifficationType)).Select(i => i.ToLower());

            var tarifficationTypes = this._dataService.GetDbSet<Translation>()
                .Where(i => tarifficationTypeNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (TarifficationType?)Enum.Parse<TarifficationType>(i.Name, true))
                .ToList();

            //DeliveryType search

            var deliveryTypeNames = Enum.GetNames(typeof(DeliveryType)).Select(i => i.ToLower());

            var deliveryTypes = this._dataService.GetDbSet<Translation>()
                .Where(i => deliveryTypeNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (DeliveryType?)Enum.Parse<DeliveryType>(i.Name, true))
                .ToList();


            var statusNames = Enum.GetNames(typeof(ShippingState)).Select(i => i.ToLower());

            var statuses = this._dataService.GetDbSet<Translation>()
                .Where(i => statusNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (ShippingState?)Enum.Parse<ShippingState>(i.Name, true))
                .ToList();

            var transportCompanies = this._dataService.GetDbSet<TransportCompany>()
                .Where(i => !string.IsNullOrEmpty(i.Title) && i.Title.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                .Select(i => i.Id).ToList();

            var vehicleTypes = this._dataService.GetDbSet<VehicleType>()
                .Where(i => i.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                .Select(i => i.Id).ToList();

            return query.Where(i =>
               columns.Contains("shippingNumber") && !string.IsNullOrEmpty(i.ShippingNumber) && i.ShippingNumber.Contains(search)
            || columns.Contains("deliveryInvoiceNumber") && !string.IsNullOrEmpty(i.DeliveryInvoiceNumber) && i.DeliveryInvoiceNumber.Contains(search)
            || columns.Contains("deviationReasonsComments") && !string.IsNullOrEmpty(i.DeviationReasonsComments) && i.DeviationReasonsComments.Contains(search)
            || columns.Contains("additionalCostsComments") && !string.IsNullOrEmpty(i.AdditionalCostsComments) && i.AdditionalCostsComments.Contains(search)
            || columns.Contains("invoiceNumber") && !string.IsNullOrEmpty(i.InvoiceNumber) && i.InvoiceNumber.Contains(search)
            || columns.Contains("temperatureMin") && isInt && i.TemperatureMin == searchInt
            || columns.Contains("temperatureMax") && isInt && i.TemperatureMax == searchInt
            || columns.Contains("palletsCount") && isInt && i.PalletsCount == searchInt
            || columns.Contains("actualPalletsCount") && isInt && i.ActualPalletsCount == searchInt
            || columns.Contains("confirmedPalletsCount") && isInt && i.ConfirmedPalletsCount == searchInt
            || columns.Contains("weightKg") && isDecimal && i.WeightKg >= searchDecimal - precision && i.WeightKg <= searchDecimal + precision
            || columns.Contains("actualWeightKg") && isDecimal && i.ActualWeightKg >= searchDecimal - precision && i.ActualWeightKg <= searchDecimal + precision
            || columns.Contains("totalDeliveryCost") && isDecimal && i.TotalDeliveryCost >= searchDecimal - precision && i.TotalDeliveryCost <= searchDecimal + precision
            || columns.Contains("otherCosts") && isDecimal && i.OtherCosts >= searchDecimal - precision && i.OtherCosts <= searchDecimal + precision
            || columns.Contains("deliveryCostWithoutVAT") && isDecimal && i.DeliveryCostWithoutVAT >= searchDecimal - precision && i.DeliveryCostWithoutVAT <= searchDecimal + precision
            || columns.Contains("returnCostWithoutVAT") && isDecimal && i.ReturnCostWithoutVAT >= searchDecimal - precision && i.ReturnCostWithoutVAT <= searchDecimal + precision
            || columns.Contains("invoiceAmountWithoutVAT") && isDecimal && i.InvoiceAmountWithoutVAT >= searchDecimal - precision && i.InvoiceAmountWithoutVAT <= searchDecimal + precision
            || columns.Contains("additionalCostsWithoutVAT") && isDecimal && i.AdditionalCostsWithoutVAT >= searchDecimal - precision && i.AdditionalCostsWithoutVAT <= searchDecimal + precision
            || columns.Contains("trucksDowntime") && isDecimal && i.TrucksDowntime >= searchDecimal - precision && i.TrucksDowntime <= searchDecimal + precision
            || columns.Contains("returnRate") && isDecimal && i.ReturnRate >= searchDecimal - precision && i.ReturnRate <= searchDecimal + precision
            || columns.Contains("additionalPointRate") && isDecimal && i.AdditionalPointRate >= searchDecimal - precision && i.AdditionalPointRate <= searchDecimal + precision
            || columns.Contains("downtimeRate") && isDecimal && i.DowntimeRate >= searchDecimal - precision && i.DowntimeRate <= searchDecimal + precision
            || columns.Contains("blankArrivalRate") && isDecimal && i.BlankArrivalRate >= searchDecimal - precision && i.BlankArrivalRate <= searchDecimal + precision
            || columns.Contains("shippingCreationDate") && i.ShippingCreationDate.HasValue && i.ShippingCreationDate.Value.ToString(searchDateFormat).Contains(search)
            || columns.Contains("loadingArrivalTime") && i.LoadingArrivalTime.HasValue && i.LoadingArrivalTime.Value.ToString(searchDateFormat).Contains(search)
            || columns.Contains("loadingDepartureTime") && i.LoadingDepartureTime.HasValue && i.LoadingDepartureTime.Value.ToString(searchDateFormat).Contains(search)
            || columns.Contains("documentsReturnDate") && i.DocumentsReturnDate.HasValue && i.DocumentsReturnDate.Value.ToString(searchDateFormat).Contains(search)
            || columns.Contains("actualDocumentsReturnDate") && i.ActualDocumentsReturnDate.HasValue && i.ActualDocumentsReturnDate.Value.ToString(searchDateFormat).Contains(search)
            || columns.Contains("tarifficationType") && tarifficationTypes.Contains(i.TarifficationType)
            || columns.Contains("deliveryType") && deliveryTypes.Contains(i.DeliveryType)
            || columns.Contains("vehicleTypeId") && vehicleTypes.Any(v => v == i.VehicleTypeId)
            || columns.Contains("carrierId") && transportCompanies.Any(t => t == i.CarrierId)
            || columns.Contains("status") && statuses.Contains(i.Status)
            );
        }
    }
}

using Application.BusinessModels.Shared.Actions;
using Application.BusinessModels.Shared.Handlers;
using Application.BusinessModels.Shippings.Handlers;
using Application.Extensions;
using Application.Services.Triggers;
using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using AutoMapper;
using DAL.Queries;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
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
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;

        public ShippingsService(
            IHistoryService historyService,
            IAuditDataService dataService,
            IUserProvider userIdProvider,
            IFieldDispatcherService fieldDispatcherService,
            IFieldPropertiesService fieldPropertiesService, 
            IServiceProvider serviceProvider, 
            ITriggersService triggersService,
            IValidationService validationService,
            IFieldSetterFactory fieldSetterFactory)
            : base(dataService, userIdProvider, fieldDispatcherService, fieldPropertiesService, serviceProvider, triggersService, validationService, fieldSetterFactory)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
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

        public override IQueryable<Shipping> ApplyRestrictions(IQueryable<Shipping> query)
        {
            var currentUserId = _userIdProvider.GetCurrentUserId();
            var user = _dataService.GetDbSet<User>().GetById(currentUserId.Value);
            
            if (user.CarrierId.HasValue)
                query = query
                    .Where(x => x.CarrierId == user.CarrierId);

            return query;
        }

        public override string GetNumber(ShippingFormDto dto)
        {
            return dto?.ShippingNumber;
        }

        public override IEnumerable<EntityStatusDto> LoadStatusData(IEnumerable<Guid> ids)
        {
            var result = _dataService.GetDbSet<Shipping>()
                .Where(x => ids.Contains(x.Id))
                .Select(x => new EntityStatusDto { Id = x.Id.ToString(), Status = x.Status.ToString() })
                .ToList();

            return result;
        }

        protected override IEnumerable<ShippingDto> FillLookupNames(IEnumerable<ShippingDto> dtos)
        {
            var carrierIds = dtos.Where(x => !string.IsNullOrEmpty(x.CarrierId?.Value))
                                 .Select(x => x.CarrierId.Value.ToGuid())
                                 .ToList();
            var carriers = _dataService.GetDbSet<TransportCompany>()
                                       .Where(x => carrierIds.Contains(x.Id))
                                       .ToDictionary(x => x.Id.ToString());

            var vehicleTypeIds = dtos.Where(x => !string.IsNullOrEmpty(x.VehicleTypeId?.Value))
                                     .Select(x => x.VehicleTypeId.Value.ToGuid())
                                     .ToList();
            var vehicleTypes = _dataService.GetDbSet<VehicleType>()
                                           .Where(x => vehicleTypeIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            var bodyTypeIds = dtos.Where(x => !string.IsNullOrEmpty(x.BodyTypeId?.Value))
                                  .Select(x => x.BodyTypeId.Value.ToGuid())
                                  .ToList();
            var bodyTypes = _dataService.GetDbSet<BodyType>()
                                        .Where(x => bodyTypeIds.Contains(x.Id))
                                        .ToDictionary(x => x.Id.ToString());

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.CarrierId?.Value)
                    && carriers.TryGetValue(dto.CarrierId.Value, out TransportCompany carrier))
                {
                    dto.CarrierId.Name = carrier.Title;
                }

                if (!string.IsNullOrEmpty(dto.VehicleTypeId?.Value)
                    && vehicleTypes.TryGetValue(dto.VehicleTypeId.Value, out VehicleType vehicleType))
                {
                    dto.VehicleTypeId.Name = vehicleType.Name;
                }

                if (!string.IsNullOrEmpty(dto.BodyTypeId?.Value)
                    && bodyTypes.TryGetValue(dto.BodyTypeId.Value, out BodyType bodyType))
                {
                    dto.BodyTypeId.Name = bodyType.Name;
                }

                yield return dto;
            }
        }

        protected override IFieldSetter<Shipping> ConfigureHandlers(IFieldSetter<Shipping> setter, ShippingFormDto dto)
        {
            return setter
                .AddHandler(e => e.CarrierId, new CarrierIdHandler(_dataService, _historyService))
                .AddHandler(e => e.PalletsCount, new PalletsCountHandler())
                .AddHandler(e => e.ActualPalletsCount, new ActualPalletsCountHandler())
                .AddHandler(e => e.ConfirmedPalletsCount, new ConfirmedPalletsCountHandler())
                .AddHandler(e => e.WeightKg, new WeightKgHandler())
                .AddHandler(e => e.ActualWeightKg, new ActualWeightKgHandler())
                .AddHandler(e => e.LoadingArrivalTime, new LoadingArrivalTimeHandler(_dataService, _historyService))
                .AddHandler(e => e.LoadingDepartureTime, new LoadingDepartureTimeHandler(_dataService, _historyService))
                .AddHandler(e => e.TotalDeliveryCost, new TotalDeliveryCostHandler())
                .AddHandler(e => e.DeliveryCostWithoutVAT, new DeliveryCostWithoutVATHandler(_historyService))
                .AddHandler(e => e.ReturnCostWithoutVAT, new ReturnCostWithoutVATHandler(_historyService))
                .AddHandler(e => e.AdditionalCostsWithoutVAT, new AdditionalCostsWithoutVATHandler(_historyService))
                .AddHandler(e => e.TrucksDowntime, new TrucksDowntimeHandler());
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ShippingDto, Shipping>()
                    .ForMember(t => t.Id, e => e.MapFrom((s) => s.Id.ToGuid()))
                    .ForMember(t => t.DeliveryType, e => e.Condition((s) => s.DeliveryType != null && !string.IsNullOrEmpty(s.DeliveryType.Value)))
                    .ForMember(t => t.DeliveryType, e => e.MapFrom((s) => MapFromStateDto<DeliveryType>(s.DeliveryType.Value)))
                    .ForMember(t => t.TarifficationType, e => e.Condition((s) => s.TarifficationType != null && !string.IsNullOrEmpty(s.TarifficationType.Value)))
                    .ForMember(t => t.TarifficationType, e => e.MapFrom((s) => MapFromStateDto<TarifficationType>(s.TarifficationType.Value)))
                    .ForMember(t => t.CarrierId, e => e.Condition((s) => s.CarrierId != null))
                    .ForMember(t => t.CarrierId, e => e.MapFrom((s) => s.CarrierId.Value.ToGuid()))
                    .ForMember(t => t.VehicleTypeId, e => e.Condition((s) => s.VehicleTypeId != null))
                    .ForMember(t => t.VehicleTypeId, e => e.MapFrom((s) => s.VehicleTypeId.Value.ToGuid()))
                    .ForMember(t => t.VehicleTypeId, e => e.Condition((s) => s.BodyTypeId != null))
                    .ForMember(t => t.BodyTypeId, e => e.MapFrom((s) => s.BodyTypeId.Value.ToGuid()))
                    .ForMember(t => t.LoadingArrivalTime, e => e.MapFrom((s) => ParseDateTime(s.LoadingArrivalTime)))
                    .ForMember(t => t.LoadingDepartureTime, e => e.MapFrom((s) => ParseDateTime(s.LoadingDepartureTime)))
                    .ForMember(t => t.BlankArrival, e => e.MapFrom((s) => s.BlankArrival.GetValueOrDefault()))
                    .ForMember(t => t.Waybill, e => e.MapFrom((s) => s.Waybill.GetValueOrDefault()))
                    .ForMember(t => t.WaybillTorg12, e => e.MapFrom((s) => s.WaybillTorg12.GetValueOrDefault()))
                    .ForMember(t => t.TransportWaybill, e => e.MapFrom((s) => s.TransportWaybill.GetValueOrDefault()))
                    .ForMember(t => t.Invoice, e => e.MapFrom((s) => s.Invoice.GetValueOrDefault()))
                    .ForMember(t => t.DocumentsReturnDate, e => e.MapFrom((s) => ParseDateTime(s.DocumentsReturnDate)))
                    .ForMember(t => t.ActualDocumentsReturnDate, e => e.MapFrom((s) => ParseDateTime(s.ActualDocumentsReturnDate)))
                    .ForMember(t => t.CostsConfirmedByShipper, e => e.MapFrom((s) => s.CostsConfirmedByShipper.GetValueOrDefault()))
                    .ForMember(t => t.CostsConfirmedByCarrier, e => e.MapFrom((s) => s.CostsConfirmedByCarrier.GetValueOrDefault()));

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
                    .ForMember(t => t.ActualDocumentsReturnDate, e => e.MapFrom((s, t) => s.ActualDocumentsReturnDate?.ToString("dd.MM.yyyy")));
            });
            return result;
        }

        public override void MapFromDtoToEntity(Shipping entity, ShippingDto dto)
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

            _mapper.Map(dto, entity);
        }

        public override void MapFromFormDtoToEntity(Shipping entity, ShippingFormDto dto)
        {
            MapFromDtoToEntity(entity, dto);
            SaveRoutePoints(entity, dto);
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

        private string GetBodyTypeNameById(Guid? id)
        {
            return id == null ? null : _dataService.GetById<BodyType>(id.Value)?.Name;
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
                            if (pointDto.IsLoading)
                            {
                                order.ShippingDate = ParseDateTime(pointDto.PlannedDate);
                                order.LoadingArrivalTime = ParseDateTime(pointDto.ArrivalTime);
                                order.LoadingDepartureTime = ParseDateTime(pointDto.DepartureTime);
                                if (!string.IsNullOrEmpty(pointDto.VehicleStatus))
                                    order.ShippingStatus = MapFromStateDto<VehicleState>(pointDto.VehicleStatus);
                            }
                            else
                            {
                                order.DeliveryDate = ParseDateTime(pointDto.PlannedDate);
                                order.UnloadingArrivalTime = ParseDateTime(pointDto.ArrivalTime);
                                order.UnloadingDepartureTime = ParseDateTime(pointDto.DepartureTime);
                                order.TrucksDowntime = pointDto.TrucksDowntime;
                                if (!string.IsNullOrEmpty(pointDto.VehicleStatus))
                                    order.DeliveryStatus = MapFromStateDto<VehicleState>(pointDto.VehicleStatus);
                            }
                        }
                    }
                }

                var loadingArrivalTime = orders.Select(i => i.LoadingArrivalTime).Where(i => i != null).Min();
                var loadingDepartureTime = orders.Select(i => i.LoadingDepartureTime).Where(i => i != null).Min();

                entity.LoadingArrivalTime = loadingArrivalTime;
                entity.LoadingDepartureTime = loadingDepartureTime;
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
                    string key = $"L-{order.ShippingWarehouseId.ToString()}-{order.ShippingDate?.ToString("dd.MM.yyyy")}";
                    if (!points.TryGetValue(key, out point))
                    {
                        point = new RoutePointDto
                        {
                            WarehouseName = _dataService.GetById<ShippingWarehouse>(order.ShippingWarehouseId.Value)?.WarehouseName,
                            Address = order.ShippingAddress,
                            PlannedDate = order.ShippingDate?.ToString("dd.MM.yyyy"),
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
                    string key = $"U-{order.DeliveryWarehouseId.ToString()}-{order.DeliveryDate?.ToString("dd.MM.yyyy")}";
                    if (!points.TryGetValue(key, out point))
                    {
                        point = new RoutePointDto
                        {
                            WarehouseName = order.ClientName,
                            Address = order.DeliveryAddress,
                            PlannedDate = order.DeliveryDate?.ToString("dd.MM.yyyy"),
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

            var pointsList = points.Values.OrderBy(p => p.PlannedDate.ParseDate())
                                          .ThenBy(p => p.IsLoading ? 0 : 1)
                                          .ThenBy(p => p.VehicleStatus)
                                          .ThenBy(p => p.WarehouseName)
                                          .ToList();
            return pointsList;
        }

        public override IQueryable<Shipping> ApplySearchForm(IQueryable<Shipping> query, FilterFormDto<ShippingFilterDto> searchForm, List<string> columns = null)
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
                         .WhereAnd(searchForm.Filter.BodyTypeId.ApplyOptionsFilter<Shipping, Guid?>(i => i.BodyTypeId, ref parameters, i => new Guid(i)))
                         .WhereAnd(searchForm.Filter.Waybill.ApplyBooleanFilter<Shipping>(i => i.Waybill, ref parameters))
                         .WhereAnd(searchForm.Filter.WaybillTorg12.ApplyBooleanFilter<Shipping>(i => i.WaybillTorg12, ref parameters))
                         .WhereAnd(searchForm.Filter.WeightKg.ApplyNumericFilter<Shipping>(i => i.WeightKg, ref parameters));

            string sql = $@"SELECT * FROM ""Shippings"" {where}";
            query = query.FromSql(sql, parameters.ToArray());

            // Apply Search
            query = this.ApplySearch(query, searchForm?.Filter?.Search, columns ?? searchForm?.Filter?.Columns);

            return query.OrderBy(searchForm.Sort?.Name, searchForm.Sort?.Desc)
                .DefaultOrderBy(i => i.ShippingCreationDate, !string.IsNullOrEmpty(searchForm.Sort?.Name))
                .DefaultOrderBy(i => i.Id, true);
        }

        private IQueryable<Shipping> ApplySearch(IQueryable<Shipping> query, string search, List<string> columns)
        {
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

        protected override ExcelMapper<ShippingDto> CreateExportExcelMapper()
        {
            string lang = _userIdProvider.GetCurrentUser()?.Language;
            return base.CreateExportExcelMapper()
                .MapColumn(w => w.CarrierId, new DictionaryReferenceExcelColumn(GetCarrierIdByName))
                .MapColumn(w => w.BodyTypeId, new DictionaryReferenceExcelColumn(GetBodyTypeIdByName))
                .MapColumn(w => w.VehicleTypeId, new DictionaryReferenceExcelColumn(GetVehicleTypeIdByName))
                .MapColumn(i => i.Status, new StateExcelColumn<ShippingState>(lang))
                .MapColumn(i => i.DeliveryType, new EnumExcelColumn<DeliveryType>(lang))
                .MapColumn(i => i.TarifficationType, new EnumExcelColumn<TarifficationType>(lang));
        }

        private Guid? GetCarrierIdByName(string name)
        {
            var entry = _dataService.GetDbSet<TransportCompany>().Where(t => t.Title == name).FirstOrDefault();
            return entry?.Id;
        }

        private Guid? GetVehicleTypeIdByName(string name)
        {
            var entry = _dataService.GetDbSet<VehicleType>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        private Guid? GetBodyTypeIdByName(string name)
        {
            var entry = _dataService.GetDbSet<BodyType>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }
    }
}

using Application.BusinessModels.Shared.Handlers;
using Application.BusinessModels.Shippings.Handlers;
using Application.Extensions;
using Application.Services.Triggers;
using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using AutoMapper;
using DAL.Extensions;
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Application.BusinessModels.Orders.Actions;
using Application.BusinessModels.Shared.Actions;
using AutoMapper.QueryableExtensions;
using Domain.Services.Orders;
using Domain.Services.Translations;
using Microsoft.EntityFrameworkCore.Internal;
using OfficeOpenXml;
using Serilog;

namespace Application.Services.Shippings
{
    public class ShippingsService :
        GridService<Shipping, ShippingDto, ShippingFormDto, ShippingSummaryDto, ShippingFilterDto>, IShippingsService
    {
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;

        private readonly IOrdersService _ordersService;

        private readonly IChangeTrackerFactory _changeTrackerFactory;

        private readonly IGroupAppAction<Order> _unionOrdersAction;


        public ShippingsService(
            IHistoryService historyService,
            ICommonDataService dataService,
            IUserProvider userIdProvider,
            IFieldDispatcherService fieldDispatcherService,
            IFieldPropertiesService fieldPropertiesService,
            IServiceProvider serviceProvider,
            ITriggersService triggersService,
            IValidationService validationService,
            IFieldSetterFactory fieldSetterFactory,
            IChangeTrackerFactory changeTrackerFactory,
            IOrdersService ordersService,
            IGroupAppAction<Order> unionOrdersAction
        )
            : base(dataService, userIdProvider, fieldDispatcherService, fieldPropertiesService, serviceProvider,
                triggersService, validationService, fieldSetterFactory)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
            _changeTrackerFactory = changeTrackerFactory;
            _ordersService = ordersService;
            _unionOrdersAction = unionOrdersAction;
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
                entities = dbSet.Where(x =>
                    x.ShippingNumber.Contains(dto.Number, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            else
            {
                entities = dbSet.Where(x => x.ShippingNumber == dto.Number).ToList();
            }

            var result = entities.Select(MapFromEntityToLookupDto);
            return result;
        }

        public IEnumerable<ShippingFormDto> FindByPoolingReservationId(NumberSearchFormDto dto)
        {
            var dbSet = _dataService.GetDbSet<Shipping>();
            List<Shipping> entities;
            if (dto.IsPartial)
            {
                entities = dbSet.Where(x =>
                    x.PoolingReservationId.Contains(dto.Number, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            else
            {
                entities = dbSet.Where(x => x.PoolingReservationId == dto.Number).ToList();
            }

            var result = entities.Select(MapFromEntityToFormDto);
            return result;
        }


        public override IQueryable<Shipping> ApplyRestrictions(IQueryable<Shipping> query)
        {
            var currentUserId = _userIdProvider.GetCurrentUserId();
            var user = _dataService.GetDbSet<User>().GetById(currentUserId.Value);

            if (user.CarrierId.HasValue)
                query = query.Where(x =>
                    x.CarrierId == user.CarrierId && x.Status != null && x.Status != ShippingState.ShippingCreated);

            if (user.ClientId.HasValue)
                query = new List<Shipping>().AsQueryable();

            if (user.ProviderId.HasValue)
                query = query.Where(x => x.ProviderId == user.ProviderId);

            return query;
        }

        public override string GetNumber(ShippingFormDto dto)
        {
            return dto?.ShippingNumber.Value;
        }

        public override IEnumerable<EntityStatusDto> LoadStatusData(IEnumerable<Guid> ids)
        {
            var result = _dataService.GetDbSet<Shipping>()
                .Where(x => ids.Contains(x.Id))
                .Select(x => new EntityStatusDto {Id = x.Id.ToString(), Status = x.Status.ToString()})
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

            var providerIds = dtos.Where(x => !string.IsNullOrEmpty(x.ProviderId?.Value))
                .Select(x => x.ProviderId.Value.ToGuid())
                .ToList();

            var providers = _dataService.GetDbSet<Provider>()
                .Where(x => providerIds.Contains(x.Id))
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

                if (!string.IsNullOrEmpty(dto.ProviderId?.Value)
                    && providers.TryGetValue(dto.ProviderId.Value, out Provider provider))
                {
                    dto.ProviderId.Name = provider.Name;
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

                if (!string.IsNullOrEmpty(dto.ShippingNumber?.Value))
                {
                    dto.ShippingNumber.Name = dto.Id;
                }

                yield return dto;
            }
        }

        private IEnumerable<ShippingOrderDto> FillLookupOrdersNames(IEnumerable<ShippingOrderDto> dtos)
        {
            var clientIds = dtos.Where(x => !string.IsNullOrEmpty(x.ClientId?.Value))
                .Select(x => x.ClientId.Value.ToGuid())
                .ToList();
            var clients = _dataService.GetDbSet<Client>()
                .Where(x => clientIds.Contains(x.Id))
                .ToDictionary(x => x.Id.ToString());

            var shippindWarehouseIds = dtos.Where(x => !string.IsNullOrEmpty(x.ShippingWarehouseId?.Value))
                .Select(x => x.ShippingWarehouseId.Value.ToGuid())
                .ToList();
            var shippingWarehouses = _dataService.GetDbSet<ShippingWarehouse>()
                .Where(x => shippindWarehouseIds.Contains(x.Id))
                .ToDictionary(x => x.Id.ToString());

            var deliveryWarehouseIds = dtos.Where(x => !string.IsNullOrEmpty(x.DeliveryWarehouseId?.Value))
                .Select(x => x.DeliveryWarehouseId.Value.ToGuid())
                .ToList();
            var deliveryWarehouses = _dataService.GetDbSet<Warehouse>()
                .Where(x => deliveryWarehouseIds.Contains(x.Id))
                .ToDictionary(x => x.Id.ToString());

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.ClientId?.Value)
                    && clients.TryGetValue(dto.ClientId.Value, out Client client))
                {
                    dto.ClientId.Name = client.Name;
                }

                if (!string.IsNullOrEmpty(dto.OrderNumber?.Value))
                {
                    dto.OrderNumber.Name = dto.Id;
                }

                if (!string.IsNullOrEmpty(dto.ShippingWarehouseId?.Value)
                    && shippingWarehouses.TryGetValue(dto.ShippingWarehouseId.Value,
                        out ShippingWarehouse shippingWarehouse))
                {
                    dto.ShippingWarehouseId.Name = shippingWarehouse.WarehouseName;
                }

                if (!string.IsNullOrEmpty(dto.DeliveryWarehouseId?.Value)
                    && deliveryWarehouses.TryGetValue(dto.DeliveryWarehouseId.Value,
                        out Warehouse deliveryWarehouse))
                {
                    dto.DeliveryWarehouseId.Name = deliveryWarehouse.WarehouseName;
                }

                yield return dto;
            }
        }

        protected override IFieldSetter<Shipping> ConfigureHandlers(IFieldSetter<Shipping> setter, ShippingFormDto dto)
        {
            return setter
                .AddHandler(e => e.CarrierId, new CarrierIdHandler(_dataService, _historyService))
                .AddHandler(e => e.PalletsCount, new PalletsCountHandler())
                .AddHandler(e => e.ConfirmedPalletsCount, new ConfirmedPalletsCountHandler())
                .AddHandler(e => e.WeightKg, new WeightKgHandler())
                .AddHandler(e => e.ConfirmedWeightKg, new ConfirmedWeightKgHandler())
                .AddHandler(e => e.LoadingArrivalTime, new LoadingArrivalTimeHandler(_dataService, _historyService))
                .AddHandler(e => e.LoadingDepartureTime, new LoadingDepartureTimeHandler(_dataService, _historyService))
                .AddHandler(e => e.TotalDeliveryCost, new TotalDeliveryCostHandler())
                .AddHandler(e => e.DeliveryCostWithoutVAT, new DeliveryCostWithoutVATHandler(_historyService))
                .AddHandler(e => e.ReturnCostWithoutVAT, new ReturnCostWithoutVATHandler(_historyService))
                .AddHandler(e => e.AdditionalCostsWithoutVAT, new AdditionalCostsWithoutVATHandler(_historyService))
                .AddHandler(e => e.TrucksDowntime, new TrucksDowntimeHandler());
        }

        protected override IChangeTracker ConfigureChangeTacker()
        {
            return _changeTrackerFactory.CreateChangeTracker()
                .TrackAll<Shipping>()
                .Remove<Shipping>(i => i.Id);
        }

        private MapperConfiguration ConfigureMapper()
        {
            var lang = _userIdProvider.GetCurrentUser()?.Language;

            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ShippingDto, Shipping>()
                    .ForMember(t => t.Id, e => e.Ignore())
                    .ForMember(t => t.ShippingNumber, e => e.MapFrom(s => s.ShippingNumber.Value))
                    .ForMember(t => t.Status, e => e.Ignore())
                    .ForMember(t => t.ManualConfirmedWeightKg, e => e.Ignore())
                    .ForMember(t => t.ManualConfirmedPalletsCount, e => e.Ignore())
                    .ForMember(t => t.ManualPalletsCount, e => e.Ignore())
                    .ForMember(t => t.ManualTotalDeliveryCost, e => e.Ignore())
                    .ForMember(t => t.ManualTrucksDowntime, e => e.Ignore())
                    .ForMember(t => t.ManualWeightKg, e => e.Ignore())
                    .ForMember(t => t.DeliveryType,
                        e => e.MapFrom((s) =>
                            s.DeliveryType == null || string.IsNullOrEmpty(s.DeliveryType.Value)
                                ? (DeliveryType?) null
                                : MapFromStateDto<DeliveryType>(s.DeliveryType.Value)))
                    .ForMember(t => t.TarifficationType,
                        e => e.MapFrom((s) =>
                            s.TarifficationType == null || string.IsNullOrEmpty(s.TarifficationType.Value)
                                ? (TarifficationType?) null
                                : MapFromStateDto<TarifficationType>(s.TarifficationType.Value)))
                    .ForMember(t => t.CarrierId,
                        e => e.MapFrom((s) => s.CarrierId == null ? null : s.CarrierId.Value.ToGuid()))
                    .ForMember(t => t.ProviderId,
                        e => e.MapFrom((s) => s.ProviderId == null ? null : s.ProviderId.Value.ToGuid()))
                    .ForMember(t => t.VehicleTypeId,
                        e => e.MapFrom((s) => s.VehicleTypeId == null ? null : s.VehicleTypeId.Value.ToGuid()))
                    .ForMember(t => t.BodyTypeId,
                        e => e.MapFrom((s) => s.BodyTypeId == null ? null : s.BodyTypeId.Value.ToGuid()))
                    .ForMember(t => t.LoadingArrivalTime, e => e.MapFrom((s) => ParseDateTime(s.LoadingArrivalTime)))
                    .ForMember(t => t.LoadingDepartureTime,
                        e => e.MapFrom((s) => ParseDateTime(s.LoadingDepartureTime)))
                    .ForMember(t => t.BlankArrival, e => e.MapFrom((s) => s.BlankArrival.GetValueOrDefault()))
                    .ForMember(t => t.Waybill, e => e.MapFrom((s) => s.Waybill.GetValueOrDefault()))
                    .ForMember(t => t.WaybillTorg12, e => e.MapFrom((s) => s.WaybillTorg12.GetValueOrDefault()))
                    .ForMember(t => t.TransportWaybill, e => e.MapFrom((s) => s.TransportWaybill.GetValueOrDefault()))
                    .ForMember(t => t.Invoice, e => e.MapFrom((s) => s.Invoice.GetValueOrDefault()))
                    .ForMember(t => t.DocumentsReturnDate, e => e.MapFrom((s) => ParseDateTime(s.DocumentsReturnDate)))
                    .ForMember(t => t.ActualDocumentsReturnDate,
                        e => e.MapFrom((s) => ParseDateTime(s.ActualDocumentsReturnDate)))
                    .ForMember(t => t.CostsConfirmedByShipper,
                        e => e.MapFrom((s) => s.CostsConfirmedByShipper.GetValueOrDefault()))
                    .ForMember(t => t.CostsConfirmedByCarrier,
                        e => e.MapFrom((s) => s.CostsConfirmedByCarrier.GetValueOrDefault()));

                cfg.CreateMap<ShippingDto, ShippingFormDto>()
                    ;

                cfg.CreateMap<Shipping, ShippingDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.PoolingReservationId, e => e.MapFrom((s, t) => s.PoolingReservationId.ToString()))
                    .ForMember(t => t.ShippingNumber,
                        e => e.MapFrom((s, t) => new LookUpDto(s.ShippingNumber, s.Id.ToString())))
                    .ForMember(t => t.Status, e => e.MapFrom((s, t) => s.Status?.ToString()?.ToLowerFirstLetter()))
                    .ForMember(t => t.PoolingStatus,
                        e => e.MapFrom((s, t) => s.PoolingState?.ToString()?.ToLowerFirstLetter()))
                    .ForMember(t => t.DeliveryType,
                        e => e.MapFrom((s, t) => s.DeliveryType == null ? null : s.DeliveryType.GetEnumLookup(lang)))
                    .ForMember(t => t.CarrierId,
                        e => e.MapFrom((s, t) => s.CarrierId == null ? null : new LookUpDto(s.CarrierId.ToString())))
                    .ForMember(t => t.ProviderId,
                        e => e.MapFrom((s, t) => s.ProviderId == null ? null : new LookUpDto(s.ProviderId.ToString())))
                    .ForMember(t => t.VehicleTypeId,
                        e => e.MapFrom((s, t) =>
                            s.VehicleTypeId == null ? null : new LookUpDto(s.VehicleTypeId.ToString())))
                    .ForMember(t => t.BodyTypeId,
                        e => e.MapFrom((s, t) => s.BodyTypeId == null ? null : new LookUpDto(s.BodyTypeId.ToString())))
                    .ForMember(t => t.TarifficationType,
                        e => e.MapFrom((s, t) =>
                            s.TarifficationType == null ? null : s.TarifficationType.GetEnumLookup(lang)))
                    .ForMember(t => t.LoadingArrivalTime,
                        e => e.MapFrom((s, t) => s.LoadingArrivalTime?.ToString("dd.MM.yyyy HH:mm")))
                    .ForMember(t => t.LoadingDepartureTime,
                        e => e.MapFrom((s, t) => s.LoadingDepartureTime?.ToString("dd.MM.yyyy HH:mm")))
                    .ForMember(t => t.DocumentsReturnDate,
                        e => e.MapFrom((s, t) => s.DocumentsReturnDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ActualDocumentsReturnDate,
                        e => e.MapFrom((s, t) => s.ActualDocumentsReturnDate?.ToString("dd.MM.yyyy")));
            });
            return result;
        }

        public override void MapFromDtoToEntity(Shipping entity, ShippingDto dto)
        {
            bool isNew = string.IsNullOrEmpty(dto.Id);

            _mapper.Map(dto, entity);

            IEnumerable<string> readOnlyFields = null;
            var currentUser = _userIdProvider.GetCurrentUser();
            
            if (!isNew)
            {
                if (currentUser != null)
                {
                    string stateName = entity.Status?.ToString()?.ToLowerFirstLetter();
                    readOnlyFields = _fieldPropertiesService.GetReadOnlyFields(FieldPropertiesForEntityType.Shippings,
                        stateName, null, currentUser.Id);
                }

                if (currentUser?.ProviderId != null)
                {
                    entity.ProviderId = entity.ProviderId ?? currentUser.ProviderId;
                }
            }
            else
            {
                InitializeNewShipping(entity, currentUser);
            }
        }

        private void InitializeNewShipping(Shipping shipping, CurrentUserDto currentUser)
        {
            shipping.Status = shipping.Status ?? ShippingState.ShippingCreated;
            shipping.ShippingCreationDate = DateTime.UtcNow;
            shipping.ShippingNumber = shipping.ShippingNumber ?? ShippingNumberProvider.GetNextShippingNumber();
            shipping.DeliveryType = DeliveryType.Delivery;
            shipping.UserCreatorId = currentUser.Id;

            if (currentUser?.CarrierId != null)
            {
                shipping.CarrierId = currentUser.CarrierId;
            }

            if (currentUser?.ProviderId != null)
            {
                shipping.ProviderId = currentUser.ProviderId;
            }
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

            return FillLookupNames(_mapper.Map<ShippingDto>(entity));
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
            var result = new List<ShippingOrderDto>();
            foreach (Order order in orders.OrderBy(o => o.OrderNumber))
            {
                ShippingOrderDto dto = new ShippingOrderDto
                {
                    Id = order.Id.ToString(),
                    OrderNumber = new LookUpDto(order.OrderNumber),
                    ClientId = new LookUpDto(order.ClientId.ToString()),
                    PalletsCount = order.PalletsCount,
                    WeightKg = order.WeightKg,
                    ClientOrderNumber = order.ClientOrderNumber,
                    ShippingWarehouseId = new LookUpDto(order.ShippingWarehouseId.ToString()),
                    DeliveryWarehouseId = new LookUpDto(order.DeliveryWarehouseId.ToString()),
                    ShippingDate = order.ShippingDate?.ToString("dd.MM.yyyy"),
                    DeliveryDate = order.DeliveryDate?.ToString("dd.MM.yyyy"),
                    Status = order.Status.ToString().ToLowerFirstLetter()
                };

                result.Add(dto);
            }

            result = FillLookupOrdersNames(result).ToList();
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
                    string key =
                        $"L-{order.ShippingWarehouseId.ToString()}-{order.ShippingDate?.ToString("dd.MM.yyyy")}";
                    if (!points.TryGetValue(key, out point))
                    {
                        point = new RoutePointDto
                        {
                            WarehouseName = _dataService.GetById<ShippingWarehouse>(order.ShippingWarehouseId.Value)
                                ?.WarehouseName,
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
                    string key =
                        $"U-{order.DeliveryWarehouseId.ToString()}-{order.DeliveryDate?.ToString("dd.MM.yyyy")}";
                    if (!points.TryGetValue(key, out point))
                    {
                        point = new RoutePointDto
                        {
                            WarehouseName = _dataService.GetById<Warehouse>(order.DeliveryWarehouseId.Value)
                                ?.WarehouseName,
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

        public override IQueryable<Shipping> ApplySearchForm(IQueryable<Shipping> query,
            FilterFormDto<ShippingFilterDto> searchForm, List<string> columns = null)
        {
            List<object> parameters = new List<object>();
            string where = string.Empty;

            where = where
                .WhereAnd(searchForm.Filter.ActualDocumentsReturnDate.ApplyDateRangeFilter<Shipping>(
                    i => i.ActualDocumentsReturnDate, ref parameters))
                .WhereAnd(searchForm.Filter.ConfirmedWeightKg.ApplyNumericFilter<Shipping>(i => i.ConfirmedWeightKg,
                    ref parameters))
                .WhereAnd(searchForm.Filter.AdditionalCostsComments.ApplyStringFilter<Shipping>(
                    i => i.AdditionalCostsComments, ref parameters))
                .WhereAnd(searchForm.Filter.AdditionalCostsWithoutVAT.ApplyNumericFilter<Shipping>(
                    i => i.AdditionalCostsWithoutVAT, ref parameters))
                .WhereAnd(searchForm.Filter.AdditionalPointRate.ApplyNumericFilter<Shipping>(i => i.AdditionalPointRate,
                    ref parameters))
                .WhereAnd(searchForm.Filter.BlankArrival.ApplyBooleanFilter<Shipping>(i => i.BlankArrival,
                    ref parameters))
                .WhereAnd(searchForm.Filter.BlankArrivalRate.ApplyNumericFilter<Shipping>(i => i.BlankArrivalRate,
                    ref parameters))
                .WhereAnd(searchForm.Filter.CarrierId.ApplyOptionsFilter<Shipping, Guid?>(i => i.CarrierId,
                    ref parameters, i => new Guid(i)))
                .WhereAnd(searchForm.Filter.ProviderId.ApplyOptionsFilter<Shipping, Guid?>(i => i.ProviderId,
                    ref parameters, i => new Guid(i)))
                .WhereAnd(searchForm.Filter.ConfirmedPalletsCount.ApplyNumericFilter<Shipping>(
                    i => i.ConfirmedPalletsCount, ref parameters))
                .WhereAnd(searchForm.Filter.CostsConfirmedByCarrier.ApplyBooleanFilter<Shipping>(
                    i => i.CostsConfirmedByCarrier, ref parameters))
                .WhereAnd(searchForm.Filter.CostsConfirmedByShipper.ApplyBooleanFilter<Shipping>(
                    i => i.CostsConfirmedByShipper, ref parameters))
                .WhereAnd(searchForm.Filter.DeliveryInvoiceNumber.ApplyStringFilter<Shipping>(
                    i => i.DeliveryInvoiceNumber, ref parameters))
                .WhereAnd(searchForm.Filter.DeliveryType.ApplyEnumFilter<Shipping, DeliveryType>(i => i.DeliveryType,
                    ref parameters))
                .WhereAnd(searchForm.Filter.DeliveryCostWithoutVAT.ApplyNumericFilter<Shipping>(
                    i => i.DeliveryCostWithoutVAT, ref parameters))
                .WhereAnd(searchForm.Filter.DeviationReasonsComments.ApplyStringFilter<Shipping>(
                    i => i.DeviationReasonsComments, ref parameters))
                .WhereAnd(searchForm.Filter.DocumentsReturnDate.ApplyDateRangeFilter<Shipping>(
                    i => i.DocumentsReturnDate, ref parameters))
                .WhereAnd(searchForm.Filter.DowntimeRate.ApplyNumericFilter<Shipping>(i => i.DowntimeRate,
                    ref parameters))
                .WhereAnd(searchForm.Filter.Invoice.ApplyBooleanFilter<Shipping>(i => i.Invoice, ref parameters))
                .WhereAnd(searchForm.Filter.InvoiceAmountWithoutVAT.ApplyNumericFilter<Shipping>(
                    i => i.InvoiceAmountWithoutVAT, ref parameters))
                .WhereAnd(searchForm.Filter.InvoiceNumber.ApplyStringFilter<Shipping>(i => i.InvoiceNumber,
                    ref parameters))
                .WhereAnd(searchForm.Filter.LoadingArrivalTime.ApplyDateRangeFilter<Shipping>(i => i.LoadingArrivalTime,
                    ref parameters))
                .WhereAnd(searchForm.Filter.LoadingDepartureTime.ApplyDateRangeFilter<Shipping>(
                    i => i.LoadingDepartureTime, ref parameters))
                .WhereAnd(searchForm.Filter.OtherCosts.ApplyNumericFilter<Shipping>(i => i.OtherCosts, ref parameters))
                .WhereAnd(searchForm.Filter.PalletsCount.ApplyNumericFilter<Shipping>(i => i.PalletsCount,
                    ref parameters))
                .WhereAnd(searchForm.Filter.ReturnCostWithoutVAT.ApplyNumericFilter<Shipping>(
                    i => i.ReturnCostWithoutVAT, ref parameters))
                .WhereAnd(searchForm.Filter.ReturnRate.ApplyNumericFilter<Shipping>(i => i.ReturnRate, ref parameters))
                .WhereAnd(searchForm.Filter.ShippingCreationDate.ApplyDateRangeFilter<Shipping>(
                    i => i.ShippingCreationDate, ref parameters))
                .WhereAnd(searchForm.Filter.ShippingNumber.ApplyStringFilter<Shipping>(i => i.ShippingNumber,
                    ref parameters))
                .WhereAnd(searchForm.Filter.VehicleNumber.ApplyStringFilter<Shipping>(i => i.VehicleNumber,
                    ref parameters))
                .WhereAnd(searchForm.Filter.Driver.ApplyStringFilter<Shipping>(i => i.Driver, ref parameters))
                .WhereAnd(searchForm.Filter.Status.ApplyEnumFilter<Shipping, ShippingState>(i => i.Status,
                    ref parameters))
                .WhereAnd(searchForm.Filter.TarifficationType.ApplyEnumFilter<Shipping, TarifficationType>(
                    i => i.TarifficationType, ref parameters))
                .WhereAnd(searchForm.Filter.TemperatureMax.ApplyNumericFilter<Shipping>(i => i.TemperatureMax,
                    ref parameters))
                .WhereAnd(searchForm.Filter.TemperatureMin.ApplyNumericFilter<Shipping>(i => i.TemperatureMin,
                    ref parameters))
                .WhereAnd(searchForm.Filter.TotalDeliveryCost.ApplyNumericFilter<Shipping>(i => i.TotalDeliveryCost,
                    ref parameters))
                .WhereAnd(searchForm.Filter.TransportWaybill.ApplyBooleanFilter<Shipping>(i => i.TransportWaybill,
                    ref parameters))
                .WhereAnd(searchForm.Filter.TrucksDowntime.ApplyNumericFilter<Shipping>(i => i.TrucksDowntime,
                    ref parameters))
                .WhereAnd(searchForm.Filter.VehicleTypeId.ApplyOptionsFilter<Shipping, Guid?>(i => i.VehicleTypeId,
                    ref parameters, i => new Guid(i)))
                .WhereAnd(searchForm.Filter.BodyTypeId.ApplyOptionsFilter<Shipping, Guid?>(i => i.BodyTypeId,
                    ref parameters, i => new Guid(i)))
                .WhereAnd(searchForm.Filter.Waybill.ApplyBooleanFilter<Shipping>(i => i.Waybill, ref parameters))
                .WhereAnd(searchForm.Filter.WaybillTorg12.ApplyBooleanFilter<Shipping>(i => i.WaybillTorg12,
                    ref parameters))
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

            search = search.ToLower().Trim();

            var isInt = int.TryParse(search, out int searchInt);
            var isDecimal = decimal.TryParse(search.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture,
                out decimal searchDecimal);
            decimal precision = 0.01M;

            var searchDateFormat = "dd.mm.yyyy HH24:MI";

            //TarifficationType search

            var tarifficationTypeNames = Enum.GetNames(typeof(TarifficationType)).Select(i => i.ToLower());

            var tarifficationTypes = this._dataService.GetDbSet<Translation>()
                .Where(i => tarifficationTypeNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (TarifficationType?) Enum.Parse<TarifficationType>(i.Name, true))
                .ToList();

            //DeliveryType search

            var deliveryTypeNames = Enum.GetNames(typeof(DeliveryType)).Select(i => i.ToLower());

            var deliveryTypes = this._dataService.GetDbSet<Translation>()
                .Where(i => deliveryTypeNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (DeliveryType?) Enum.Parse<DeliveryType>(i.Name, true))
                .ToList();


            var statusNames = Enum.GetNames(typeof(ShippingState)).Select(i => i.ToLower());

            var statuses = this._dataService.GetDbSet<Translation>()
                .Where(i => statusNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (ShippingState?) Enum.Parse<ShippingState>(i.Name, true))
                .ToList();


            var transportCompanies =
                _dataService.GetDbSet<TransportCompany>().Where(i => i.Title.ToLower().Contains(search));

            var providers = _dataService.GetDbSet<Provider>().Where(i => i.Name.ToLower().Contains(search));

            var vehicleTypes = _dataService.GetDbSet<VehicleType>().Where(i => i.Name.ToLower().Contains(search));

            return query.Where(i =>
                columns.Contains("shippingNumber") && !string.IsNullOrEmpty(i.ShippingNumber) &&
                i.ShippingNumber.ToLower().Contains(search)
                || columns.Contains("deliveryInvoiceNumber") && !string.IsNullOrEmpty(i.DeliveryInvoiceNumber) &&
                i.DeliveryInvoiceNumber.ToLower().Contains(search)
                || columns.Contains("deviationReasonsComments") && !string.IsNullOrEmpty(i.DeviationReasonsComments) &&
                i.DeviationReasonsComments.ToLower().Contains(search)
                || columns.Contains("additionalCostsComments") && !string.IsNullOrEmpty(i.AdditionalCostsComments) &&
                i.AdditionalCostsComments.ToLower().Contains(search)
                || columns.Contains("invoiceNumber") && !string.IsNullOrEmpty(i.InvoiceNumber) &&
                i.InvoiceNumber.ToLower().Contains(search)
                || columns.Contains("temperatureMin") && isInt && i.TemperatureMin == searchInt
                || columns.Contains("temperatureMax") && isInt && i.TemperatureMax == searchInt
                || columns.Contains("palletsCount") && isInt && i.PalletsCount == searchInt
                || columns.Contains("confirmedPalletsCount") && isInt && i.ConfirmedPalletsCount == searchInt
                || columns.Contains("weightKg") && isDecimal && i.WeightKg >= searchDecimal - precision &&
                i.WeightKg <= searchDecimal + precision
                || columns.Contains("confirmedWeightKg") && isDecimal &&
                i.ConfirmedWeightKg >= searchDecimal - precision &&
                i.ConfirmedWeightKg <= searchDecimal + precision
                || columns.Contains("totalDeliveryCost") && isDecimal &&
                i.TotalDeliveryCost >= searchDecimal - precision && i.TotalDeliveryCost <= searchDecimal + precision
                || columns.Contains("otherCosts") && isDecimal && i.OtherCosts >= searchDecimal - precision &&
                i.OtherCosts <= searchDecimal + precision
                || columns.Contains("deliveryCostWithoutVAT") && isDecimal &&
                i.DeliveryCostWithoutVAT >= searchDecimal - precision &&
                i.DeliveryCostWithoutVAT <= searchDecimal + precision
                || columns.Contains("returnCostWithoutVAT") && isDecimal &&
                i.ReturnCostWithoutVAT >= searchDecimal - precision &&
                i.ReturnCostWithoutVAT <= searchDecimal + precision
                || columns.Contains("invoiceAmountWithoutVAT") && isDecimal &&
                i.InvoiceAmountWithoutVAT >= searchDecimal - precision &&
                i.InvoiceAmountWithoutVAT <= searchDecimal + precision
                || columns.Contains("additionalCostsWithoutVAT") && isDecimal &&
                i.AdditionalCostsWithoutVAT >= searchDecimal - precision &&
                i.AdditionalCostsWithoutVAT <= searchDecimal + precision
                || columns.Contains("trucksDowntime") && isDecimal && i.TrucksDowntime >= searchDecimal - precision &&
                i.TrucksDowntime <= searchDecimal + precision
                || columns.Contains("returnRate") && isDecimal && i.ReturnRate >= searchDecimal - precision &&
                i.ReturnRate <= searchDecimal + precision
                || columns.Contains("additionalPointRate") && isDecimal &&
                i.AdditionalPointRate >= searchDecimal - precision && i.AdditionalPointRate <= searchDecimal + precision
                || columns.Contains("downtimeRate") && isDecimal && i.DowntimeRate >= searchDecimal - precision &&
                i.DowntimeRate <= searchDecimal + precision
                || columns.Contains("blankArrivalRate") && isDecimal &&
                i.BlankArrivalRate >= searchDecimal - precision && i.BlankArrivalRate <= searchDecimal + precision
                || columns.Contains("shippingCreationDate") &&
                i.ShippingCreationDate.Value.SqlFormat(searchDateFormat).Contains(search)
                || columns.Contains("loadingArrivalTime") &&
                i.LoadingArrivalTime.Value.SqlFormat(searchDateFormat).Contains(search)
                || columns.Contains("loadingDepartureTime") &&
                i.LoadingDepartureTime.Value.SqlFormat(searchDateFormat).Contains(search)
                || columns.Contains("documentsReturnDate") &&
                i.DocumentsReturnDate.Value.SqlFormat(searchDateFormat).Contains(search)
                || columns.Contains("actualDocumentsReturnDate") &&
                i.ActualDocumentsReturnDate.Value.SqlFormat(searchDateFormat).Contains(search)
                || columns.Contains("tarifficationType") && tarifficationTypes.Contains(i.TarifficationType)
                || columns.Contains("deliveryType") && deliveryTypes.Contains(i.DeliveryType)
                || columns.Contains("vehicleTypeId") && vehicleTypes.Any(v => v.Id == i.VehicleTypeId)
                || columns.Contains("carrierId") && transportCompanies.Any(t => t.Id == i.CarrierId)
                || columns.Contains("providerId") && providers.Any(t => t.Id == i.ProviderId)
                || columns.Contains("status") && statuses.Contains(i.Status)
                || columns.Contains("driver") && i.Driver.Contains(search)
                || columns.Contains("vehicleNumber") && i.VehicleNumber.Contains(search)
            );
        }

        protected override ExcelMapper<ShippingDto> CreateExportExcelMapper()
        {
            string lang = _userIdProvider.GetCurrentUser()?.Language;
            var mapper = base.CreateExportExcelMapper();
            return mapper
                .MapColumn(w => w.CarrierId, new DictionaryReferenceExcelColumn(GetCarrierIdByName))
                .MapColumn(w => w.ProviderId, new DictionaryReferenceExcelColumn(GetIdByName<Provider>))
                .MapColumn(w => w.BodyTypeId, new DictionaryReferenceExcelColumn(GetIdByName<BodyType>))
                .MapColumn(w => w.VehicleTypeId, new DictionaryReferenceExcelColumn(GetIdByName<VehicleType>))
                .MapColumn(i => i.Status, new StateExcelColumn<ShippingState>(lang))
                .MapColumn(i => i.PoolingStatus, new StateExcelColumn<ShippingPoolingState>(lang))
                .MapColumn(i => i.DeliveryType, new EnumExcelColumn<DeliveryType>(lang))
                .MapColumn(i => i.TarifficationType, new EnumExcelColumn<TarifficationType>(lang));
        }


        protected ExcelDoubleMapper<ShippingDto, ShippingFormDto, ShippingOrderDto> CreateExportDoubleExcelMapper()
        {
            string lang = _userIdProvider.GetCurrentUser()?.Language;
            var mapper =
                new ExcelDoubleMapper<ShippingDto, ShippingFormDto, ShippingOrderDto>(_dataService, _userIdProvider,
                    _fieldDispatcherService, "Orders");
            mapper = ExpandExportExcelMapperByOrders(mapper, lang);

            mapper
                .MapColumn(w => w.CarrierId, new DictionaryReferenceExcelColumn(GetCarrierIdByName))
                .MapColumn(w => w.ProviderId, new DictionaryReferenceExcelColumn(GetIdByName<Provider>))
                .MapColumn(w => w.BodyTypeId, new DictionaryReferenceExcelColumn(GetIdByName<BodyType>))
                .MapColumn(w => w.VehicleTypeId, new DictionaryReferenceExcelColumn(GetIdByName<VehicleType>))
                .MapColumn(i => i.Status, new StateExcelColumn<ShippingState>(lang))
                .MapColumn(i => i.PoolingStatus, new StateExcelColumn<ShippingPoolingState>(lang))
                .MapColumn(i => i.DeliveryType, new EnumExcelColumn<DeliveryType>(lang))
                .MapColumn(i => i.TarifficationType, new EnumExcelColumn<TarifficationType>(lang));

            mapper
                .MapInnerColumn(o => o.ClientId, new DictionaryReferenceExcelColumn(GetIdByName<Client>))
                .MapInnerColumn(o => o.DeliveryWarehouseId,
                    new DictionaryReferenceExcelColumn(GetDeliveryWarehouseIdByName))
                .MapInnerColumn(o => o.ShippingWarehouseId,
                    new DictionaryReferenceExcelColumn(GetShippingWarehouseIdByName));

            return mapper;
        }


        public Stream ExportFormsToExcel(ExportExcelFormDto<ShippingFilterDto> dto)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var excel = new ExcelPackage();

            var user = _userIdProvider.GetCurrentUser();

            string entityName = typeof(Shipping).Name.Pluralize().ToLowerFirstLetter();
            string entityDisplayName = entityName.Translate(user.Language);
            var workSheet = excel.Workbook.Worksheets.Add(entityDisplayName);

            var dbSet = _dataService.GetDbSet<Shipping>();
            var query = this.ApplySearchForm(dbSet, dto, dto.Columns);

            query = ApplyRestrictions(query);

            Log.Information("{entityName}.ExportToExcel (Load from DB): {ElapsedMilliseconds}ms", entityName,
                sw.ElapsedMilliseconds);
            sw.Restart();

            var entities = query.ToList();
            var dtos = entities.Select(MapFromEntityToFormDto);
            Log.Information("{entityName}.ExportToExcel (Convert to DTO): {ElapsedMilliseconds}ms", entityName,
                sw.ElapsedMilliseconds);
            sw.Restart();

            var excelMapper = CreateExportDoubleExcelMapper();

            var columns =
                dto.Columns.Select(c => typeof(ShippingDto).Name.ToLower() + "_" + c.ToString().ToLower());

            var innerColumns =
                dto.InnerColumns.Select(c => typeof(ShippingOrderDto).Name.ToLower() + "_" + c.ToString().ToLower());

            excelMapper.FillSheet(workSheet, dtos, user.Language, columns.Concat(innerColumns).ToList());
            Log.Information("{entityName}.ExportToExcel (Fill file): {ElapsedMilliseconds}ms", entityName,
                sw.ElapsedMilliseconds);

            return new MemoryStream(excel.GetAsByteArray());
        }

        public ImportResultDto ImportFormsFromExcel(Stream fileStream)
        {
            string entityName = typeof(Shipping).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var excel = new ExcelPackage(fileStream);
            var workSheet = excel.Workbook.Worksheets[0]; //.ElementAt(0);

            var excelMapper = CreateExportDoubleExcelMapper();
            var dtos = excelMapper.LoadEntries(workSheet);
            Log.Information("{entityName}.ImportFromExcel (Load from file): {ElapsedMilliseconds}ms", entityName,
                sw.ElapsedMilliseconds);
            sw.Restart();

            var user = _userIdProvider.GetCurrentUser();

            var importResult = ImportShippingsWidthOrders(dtos.Select(x => x.Data), user);
            Log.Information("{entityName}.ImportFromExcel (Import): {ElapsedMilliseconds}ms", entityName,
                sw.ElapsedMilliseconds);


            StringBuilder sb = new StringBuilder();
            foreach (var validateResult in importResult.Where(x => x.IsError))
            {
                sb.AppendLine($"{importResult.IndexOf(validateResult) + 2} строка: {validateResult.Error}");
            }

            if (!importResult.Any(x => x.IsError))
                return new ImportResultDto
                {
                    Message = $"{importResult.Count()} загружено"
                };

            return new ImportResultDto
            {
                Message = sb.ToString()
            };
        }

        private IEnumerable<ValidateResult> ImportShippingsWidthOrders(IEnumerable<ShippingFormDto> entitiesFrom,
            CurrentUserDto currentUser)
        {
            string entityName = typeof(ShippingFormDto).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var result = new List<ValidateResult>();

            foreach (var dto in entitiesFrom)
            {
                var validateResult = ValidateDto(dto);

                if (validateResult.IsError)
                {
                    result.Add(validateResult);
                }
                else
                {
                    var shippingAddResult = SaveOrCreate(dto);

                    result.Add(shippingAddResult);

                    if (shippingAddResult.IsError) continue;

                    if (dto.Orders == null) continue;

                    var shippingOrdersIds = new List<Guid>();

                    foreach (var innerEntry in dto.Orders)
                    {
                        OrderFormDto shippingOrder = _ordersService.MapFromShippingOrderDtoToFormDto(innerEntry);

                        shippingOrder.ShippingId = shippingAddResult.Id;

                        var res = _ordersService.SaveOrCreate(shippingOrder);

                        result.Add(res);

                        if (res.IsError) continue;

                        shippingOrdersIds.Add(Guid.Parse(res.Id));
                    }

                    _ordersService.InvokeAction("unionOrdersInExisted", shippingOrdersIds);
                    //_unionOrdersAction.Run(currentUser, shippingOrders);
                }
            }

            Log.Information("{entityName}.Import: {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }

        private ExcelDoubleMapper<ShippingDto, ShippingFormDto, ShippingOrderDto> ExpandExportExcelMapperByOrders(
            ExcelDoubleMapper<ShippingDto, ShippingFormDto, ShippingOrderDto> mapper, string lang)
        {
            Type type = typeof(ShippingOrderDto);

            var t = _fieldDispatcherService.GetDtoFields<ShippingOrderDto>();

            t.Where(f => f.FieldType != FieldType.Enum && f.FieldType != FieldType.State).ToList()
                .Select(f => new BaseExcelColumn {Property = type.GetProperty(f.Name), Field = f, Language = lang})
                .ToList()
                .ForEach(mapper.AddInnerColumn);
            return mapper;
        }

        private Guid? GetCarrierIdByName(string name)
        {
            var entry = _dataService.GetDbSet<TransportCompany>().FirstOrDefault(t => t.Title == name);
            return entry?.Id;
        }

        private Guid? GetDeliveryWarehouseIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Warehouse>().FirstOrDefault(t => t.WarehouseName == name);
            return entry?.Id;
        }

        private Guid? GetShippingWarehouseIdByName(string name)
        {
            var entry = _dataService.GetDbSet<ShippingWarehouse>().FirstOrDefault(t => t.WarehouseName == name);
            return entry?.Id;
        }

        private Guid? GetIdByName<T>(string name) where T : class, IPersistableWithName
        {
            var entry = _dataService.GetDbSet<T>().FirstOrDefault(t => t.Name == name);
            return entry?.Id;
        }
    }
}
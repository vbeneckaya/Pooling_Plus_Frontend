using Application.BusinessModels.Orders.Handlers;
using Application.BusinessModels.Shared.Handlers;
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
using Domain.Services.Orders;
using Domain.Services.UserProvider;
using Domain.Shared;
using Domain.Shared.FormFilters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Application.Services.Orders
{
    public class OrdersService : GridService<Order, OrderDto, OrderFormDto, OrderSummaryDto, OrderFilterDto>, IOrdersService
    {
        private readonly IHistoryService _historyService;

        private readonly IChangeTrackerFactory _changeTrackerFactory;

        public OrdersService(
            IHistoryService historyService,
            ICommonDataService dataService,
            IUserProvider userIdProvider,
            IFieldDispatcherService fieldDispatcherService,
            IFieldPropertiesService fieldPropertiesService, 
            IServiceProvider serviceProvider,
            ITriggersService triggersService,
            IValidationService validationService,
            IFieldSetterFactory fieldSetterFactory,
            IChangeTrackerFactory changeTrackerFactory)
            : base(dataService, userIdProvider, fieldDispatcherService, fieldPropertiesService, serviceProvider, triggersService, validationService, fieldSetterFactory)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
            _changeTrackerFactory = changeTrackerFactory;
        }

        public override OrderSummaryDto GetSummary(IEnumerable<Guid> ids)
        {
            var orders = this._dataService.GetDbSet<Order>().Where(o => ids.Contains(o.Id)).ToList();
            var result = new OrderSummaryDto
            {
                Count = orders.Count,
                BoxesCount = orders.Sum(o => o.BoxesCount ?? 0),
                PalletsCount = orders.Sum(o => o.PalletsCount ?? 0),
                WeightKg = orders.Sum(o => o.WeightKg ?? 0M)
            };
            return result;
        }

        protected override IChangeTracker ConfigureChangeTacker()
        {
            return _changeTrackerFactory.CreateChangeTracker()
                .TrackAll<Order>()
                .Remove<Order>(i => i.Id)
                .Remove<Order>(i => i.Status)
                .Remove<Order>(i => i.Source);
        }

        public IEnumerable<LookUpDto> FindByNumber(NumberSearchFormDto dto)
        {
            var dbSet = _dataService.GetDbSet<Order>();
            List<Order> entities;
            if (dto.IsPartial)
            {
                entities = dbSet.Where(x => x.OrderNumber.Contains(dto.Number, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            else
            {
                entities = dbSet.Where(x => x.OrderNumber == dto.Number).ToList();
            }
            var result = entities.Select(MapFromEntityToLookupDto);
            return result;
        }
        
        public override IQueryable<Order> ApplyRestrictions(IQueryable<Order> query)
        {
            var currentUserId = _userIdProvider.GetCurrentUserId();
            var user = _dataService.GetDbSet<User>().GetById(currentUserId.Value);
            
            if (user.CarrierId.HasValue) 
                query = query.Where(x => x.CarrierId == user.CarrierId);

            return query;
        }        
        
        public override string GetNumber(OrderFormDto dto)
        {
            return dto?.OrderNumber;
        }

        public OrderFormDto GetFormByNumber(string orderNumber)
        {
            var entity = _dataService.GetDbSet<Order>().Where(x => x.OrderNumber == orderNumber).FirstOrDefault();
            return MapFromEntityToFormDto(entity);
        }

        public override IEnumerable<EntityStatusDto> LoadStatusData(IEnumerable<Guid> ids)
        {
            var result = _dataService.GetDbSet<Order>()
                            .Where(x => ids.Contains(x.Id))
                            .Select(x => new EntityStatusDto { Id = x.Id.ToString(), Status = x.Status.ToString() })
                            .ToList();
            return result;
        }

        protected override IFieldSetter<Order> ConfigureHandlers(IFieldSetter<Order> setter, OrderFormDto dto)
        {
            bool isInjection = dto.AdditionalInfo?.Contains("INJECTION") ?? false;

            return setter
                .AddHandler(e => e.ShippingWarehouseId, new ShippingWarehouseHandler(_dataService, _historyService))
                .AddHandler(e => e.ShippingStatus, new ShippingStatusHandler(_dataService, _historyService))
                .AddHandler(e => e.DeliveryStatus, new DeliveryStatusHandler(_dataService, _historyService))
                .AddHandler(e => e.ClientOrderNumber, new ClientOrderNumberHandler(_historyService))
                .AddHandler(e => e.SoldTo, new SoldToHandler(_dataService, _historyService))
                .AddHandler(e => e.ShippingDate, new ShippingDateHandler(_dataService, _historyService, !isInjection))
                .AddHandler(e => e.DeliveryDate, new DeliveryDateHandler(_dataService, _historyService, isInjection))
                .AddHandler(e => e.OrderNumber, new OrderNumberHandler(_userIdProvider, _dataService))
                .AddHandler(e => e.ArticlesCount, new ArticlesCountHandler())
                .AddHandler(e => e.BoxesCount, new BoxesCountHandler())
                .AddHandler(e => e.ConfirmedBoxesCount, new ConfirmedBoxesCountHandler())
                .AddHandler(e => e.PalletsCount, new PalletsCountHandler(_dataService, _historyService, !isInjection))
                .AddHandler(e => e.ConfirmedPalletsCount, new ConfirmedPalletsCountHandler(_dataService, _historyService))
                .AddHandler(e => e.ActualPalletsCount, new ActualPalletsCountHandler(_dataService, _historyService))
                .AddHandler(e => e.WeightKg, new WeightKgHandler(_dataService, _historyService))
                .AddHandler(e => e.ActualWeightKg, new ActualWeightKgHandler(_dataService, _historyService))
                .AddHandler(e => e.OrderAmountExcludingVAT, new OrderAmountExcludingVATHandler())
                .AddHandler(e => e.ClientAvisationTime, new ClientAvisationTimeHandler(!isInjection))
                .AddHandler(e => e.PickingTypeId, new PickingTypeHandler(!isInjection))
                .AddHandler(e => e.LoadingArrivalTime, new LoadingArrivalTimeHandler(_dataService, _historyService))
                .AddHandler(e => e.LoadingDepartureTime, new LoadingDepartureTimeHandler(_dataService, _historyService))
                .AddHandler(e => e.UnloadingArrivalTime, new UnloadingArrivalTimeHandler(_dataService, _historyService))
                .AddHandler(e => e.UnloadingDepartureTime, new UnloadingDepartureTimeHandler(_dataService, _historyService))
                .AddHandler(e => e.TrucksDowntime, new TrucksDowntimeHandler(_dataService, _historyService))
                .AddHandler(e => e.DeliveryCost, new DeliveryCostHandler(!isInjection));
        }

        private MapperConfiguration ConfigureMapper()
        {
            var lang = _userIdProvider.GetCurrentUser()?.Language;

            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrderDto, OrderFormDto>();

                cfg.CreateMap<OrderDto, Order>()
                    .ForMember(t => t.Id, e => e.Ignore())
                    .ForMember(t => t.Status, e => e.Ignore())
                    .ForMember(t => t.DeliveryRegion, e => e.Ignore())
                    .ForMember(t => t.ManualClientAvisationTime, e => e.Ignore())
                    .ForMember(t => t.ManualDeliveryCost, e => e.Ignore())
                    .ForMember(t => t.ManualDeliveryDate, e => e.Ignore())
                    .ForMember(t => t.ManualPalletsCount, e => e.Ignore())
                    .ForMember(t => t.ManualPickingTypeId, e => e.Ignore())
                    .ForMember(t => t.ManualShippingDate, e => e.Ignore())
                    .ForMember(t => t.ShippingId, e => e.Ignore())
                    .ForMember(t => t.ShippingNumber, e => e.Ignore())
                    .ForMember(t => t.OrderShippingStatus, e => e.Ignore())
                    .ForMember(t => t.DeliveryCity, e => e.Condition((s) => s.DeliveryCity != null))
                    .ForMember(t => t.DeliveryCity, e => e.MapFrom(s => s.DeliveryCity.Value))
                    .ForMember(t => t.ShippingCity, e => e.Condition((s) => s.ShippingCity != null))
                    .ForMember(t => t.ShippingCity, e => e.MapFrom(s => s.ShippingCity.Value))
                    .ForMember(t => t.ShippingWarehouseId, e => e.Condition((s) => s.ShippingWarehouseId != null))
                    .ForMember(t => t.ShippingWarehouseId, e => e.MapFrom((s) => s.ShippingWarehouseId.Value.ToGuid()))
                    .ForMember(t => t.SoldTo, e => e.Condition((s) => s.SoldTo != null))
                    .ForMember(t => t.SoldTo, e => e.MapFrom((s) => s.SoldTo.Value))
                    .ForMember(t => t.ClientName, e => e.Condition((s) => s.ClientName != null))
                    .ForMember(t => t.ClientName, e => e.MapFrom((s) => s.ClientName.Value))
                    .ForMember(t => t.ShippingStatus, e => e.Condition((s) => !string.IsNullOrEmpty(s.ShippingStatus)))
                    .ForMember(t => t.ShippingStatus, e => e.MapFrom((s) => MapFromStateDto<VehicleState>(s.ShippingStatus)))
                    .ForMember(t => t.DeliveryStatus, e => e.Condition((s) => !string.IsNullOrEmpty(s.DeliveryStatus)))
                    .ForMember(t => t.DeliveryStatus, e => e.MapFrom((s) => MapFromStateDto<VehicleState>(s.DeliveryStatus)))
                    .ForMember(t => t.CarrierId, e => e.Condition((s) => s.CarrierId != null))
                    .ForMember(t => t.CarrierId, e => e.MapFrom((s) => s.CarrierId.Value.ToGuid()))
                    .ForMember(t => t.DeliveryType, e => e.Condition((s) => s.DeliveryType != null && !string.IsNullOrEmpty(s.DeliveryType.Value)))
                    .ForMember(t => t.DeliveryType, e => e.MapFrom((s) => MapFromStateDto<DeliveryType>(s.DeliveryType.Value)))
                    .ForMember(t => t.OrderDate, e => e.MapFrom((s) => ParseDateTime(s.OrderDate)))
                    .ForMember(t => t.OrderType, e => e.Condition((s) => s.OrderType != null && !string.IsNullOrEmpty(s.OrderType.Value)))
                    .ForMember(t => t.OrderType, e => e.MapFrom((s) => MapFromStateDto<OrderType>(s.OrderType.Value)))
                    .ForMember(t => t.ShippingDate, e => e.MapFrom((s) => ParseDateTime(s.ShippingDate)))
                    .ForMember(t => t.DeliveryDate, e => e.MapFrom((s) => ParseDateTime(s.DeliveryDate)))
                    .ForMember(t => t.BoxesCount, e => e.MapFrom((s) => Round(s.BoxesCount, 1)))
                    .ForMember(t => t.ConfirmedBoxesCount, e => e.MapFrom((s) => Round(s.ConfirmedBoxesCount, 1)))
                    .ForMember(t => t.ShippingAvisationTime, e => e.MapFrom((s) => ParseTime(s.ShippingAvisationTime)))
                    .ForMember(t => t.ClientAvisationTime, e => e.MapFrom((s) => ParseTime(s.ClientAvisationTime)))
                    .ForMember(t => t.PickingTypeId, e => e.Condition((s) => s.PickingTypeId != null))
                    .ForMember(t => t.PickingTypeId, e => e.MapFrom((s) => s.PickingTypeId.Value.ToGuid()))
                    .ForMember(t => t.LoadingArrivalTime, e => e.MapFrom((s) => ParseDateTime(s.LoadingArrivalTime)))
                    .ForMember(t => t.LoadingDepartureTime, e => e.MapFrom((s) => ParseDateTime(s.LoadingDepartureTime)))
                    .ForMember(t => t.UnloadingArrivalTime, e => e.MapFrom((s) => ParseDateTime(s.UnloadingArrivalTime)))
                    .ForMember(t => t.UnloadingDepartureTime, e => e.MapFrom((s) => ParseDateTime(s.UnloadingDepartureTime)))
                    .ForMember(t => t.DocumentsReturnDate, e => e.MapFrom((s) => ParseDateTime(s.DocumentsReturnDate)))
                    .ForMember(t => t.ActualDocumentsReturnDate, e => e.MapFrom((s) => ParseDateTime(s.ActualDocumentsReturnDate)))
                    .ForMember(t => t.PlannedReturnDate, e => e.MapFrom((s) => ParseDateTime(s.PlannedReturnDate)))
                    .ForMember(t => t.ActualReturnDate, e => e.MapFrom((s) => ParseDateTime(s.ActualReturnDate)))
                    .ForMember(t => t.DocumentReturnStatus, e => e.MapFrom((s) => s.DocumentReturnStatus.GetValueOrDefault()))
                    .ForMember(t => t.VehicleTypeId, e => e.Condition((s) => s.VehicleTypeId != null))
                    .ForMember(t => t.VehicleTypeId, e => e.MapFrom((s) => s.VehicleTypeId.Value.ToGuid()))
                    .ForMember(t => t.TarifficationType, e => e.Condition((s) => s.TarifficationType != null && !string.IsNullOrEmpty(s.TarifficationType.Value)))
                    .ForMember(t => t.TarifficationType, e => e.MapFrom((s) => MapFromStateDto<TarifficationType>(s.TarifficationType.Value)));

                cfg.CreateMap<Order, OrderDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.Status, e => e.MapFrom((s, t) => s.Status.ToString().ToLowerFirstLetter()))
                    .ForMember(t => t.OrderType, e => e.MapFrom((s, t) => s.OrderType == null ? null : s.OrderType.GetEnumLookup(lang)))
                    .ForMember(t => t.OrderDate, e => e.MapFrom((s, t) => s.OrderDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ShippingStatus, e => e.MapFrom((s, t) => s.ShippingStatus.ToString().ToLowerFirstLetter()))
                    .ForMember(t => t.DeliveryStatus, e => e.MapFrom((s, t) => s.DeliveryStatus.ToString().ToLowerFirstLetter()))
                    .ForMember(t => t.OrderShippingStatus, e => e.MapFrom((s, t) => s.OrderShippingStatus?.ToString()?.ToLowerFirstLetter()))
                    .ForMember(t => t.PickingTypeId, e => e.MapFrom((s, t) => s.PickingTypeId == null ? null : new LookUpDto(s.PickingTypeId.ToString())))
                    .ForMember(t => t.ShippingWarehouseId, e => e.MapFrom((s, t) => s.ShippingWarehouseId == null ? null : new LookUpDto(s.ShippingWarehouseId.ToString())))
                    .ForMember(t => t.SoldTo, e => e.MapFrom((s, t) => string.IsNullOrEmpty(s.SoldTo) ? null : new LookUpDto(s.SoldTo)))
                    .ForMember(t => t.ClientName, e => e.MapFrom((s, t) => string.IsNullOrEmpty(s.ClientName) ? null : new LookUpDto(s.ClientName)))
                    .ForMember(t => t.ShippingCity, e => e.MapFrom((s, t) => string.IsNullOrEmpty(s.ShippingCity) ? null : new LookUpDto(s.ShippingCity)))
                    .ForMember(t => t.DeliveryCity, e => e.MapFrom((s, t) => string.IsNullOrEmpty(s.DeliveryCity) ? null : new LookUpDto(s.DeliveryCity)))
                    .ForMember(t => t.ShippingDate, e => e.MapFrom((s, t) => s.ShippingDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.DeliveryDate, e => e.MapFrom((s, t) => s.DeliveryDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.LoadingArrivalTime, e => e.MapFrom((s, t) => s.LoadingArrivalTime?.ToString("dd.MM.yyyy HH:mm")))
                    .ForMember(t => t.LoadingDepartureTime, e => e.MapFrom((s, t) => s.LoadingDepartureTime?.ToString("dd.MM.yyyy HH:mm")))
                    .ForMember(t => t.UnloadingArrivalDate, e => e.MapFrom((s, t) => s.UnloadingArrivalTime?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.UnloadingArrivalTime, e => e.MapFrom((s, t) => s.UnloadingArrivalTime?.ToString("HH:mm")))
                    .ForMember(t => t.UnloadingDepartureDate, e => e.MapFrom((s, t) => s.UnloadingDepartureTime?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.UnloadingDepartureTime, e => e.MapFrom((s, t) => s.UnloadingDepartureTime?.ToString("HH:mm")))
                    .ForMember(t => t.DocumentsReturnDate, e => e.MapFrom((s, t) => s.DocumentsReturnDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ActualDocumentsReturnDate, e => e.MapFrom((s, t) => s.ActualDocumentsReturnDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.PlannedReturnDate, e => e.MapFrom((s, t) => s.PlannedReturnDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ActualReturnDate, e => e.MapFrom((s, t) => s.ActualReturnDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ShippingAvisationTime, e => e.MapFrom((s, t) => s.ShippingAvisationTime?.ToString(@"hh\:mm")))
                    .ForMember(t => t.ClientAvisationTime, e => e.MapFrom((s, t) => s.ClientAvisationTime?.ToString(@"hh\:mm")))
                    .ForMember(t => t.CarrierId, e => e.MapFrom((s, t) => s.CarrierId == null ? null : new LookUpDto(s.CarrierId.ToString())))
                    .ForMember(t => t.DeliveryType, e => e.MapFrom((s, t) => s.DeliveryType == null ? null : s.DeliveryType.GetEnumLookup(lang)))
                    .ForMember(t => t.VehicleTypeId, e => e.MapFrom((s, t) => s.VehicleTypeId == null ? null : new LookUpDto(s.VehicleTypeId.ToString())))
                    .ForMember(t => t.TarifficationType, e => e.MapFrom((s, t) => s.TarifficationType == null ? null : s.TarifficationType.GetEnumLookup(lang)));

                cfg.CreateMap<OrderItem, OrderItemDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()));
            });
            return result;
        }

        protected override DetailedValidationResult ValidateDto(OrderDto dto)
        {
            var lang = _userIdProvider.GetCurrentUser()?.Language;

            var result = base.ValidateDto(dto);

            var deliveryDate = dto.DeliveryDate.ToDate();
            var shippingDate = dto.ShippingDate.ToDate();

            if (deliveryDate.HasValue && shippingDate.HasValue && shippingDate > deliveryDate)
            {
                result.AddError(nameof(dto.DeliveryDate), "InvalidDeliveryOrShippingDate".Translate(lang), ValidationErrorType.InvalidDateRange);
            }

            var loadingDepartureTime = dto.LoadingDepartureTime.ToDateTime();
            var loadingArrivalTime = dto.LoadingArrivalTime.ToDateTime();

            if (loadingDepartureTime.HasValue && loadingArrivalTime.HasValue && loadingDepartureTime < loadingArrivalTime)
            {
                result.AddError(nameof(dto.LoadingArrivalTime), "InvalidLoadingDepartureOrArrivalTime".Translate(lang), ValidationErrorType.InvalidDateRange);
            }

            var unloadingDepartureTime = dto.UnloadingDepartureTime.ToDateTime();
            var unloadingArrivalTime = dto.UnloadingArrivalTime.ToDateTime();

            if (loadingDepartureTime.HasValue && unloadingArrivalTime.HasValue && unloadingArrivalTime < loadingDepartureTime)
            {
                result.AddError(nameof(dto.UnloadingArrivalTime), "InvalidUnloadingArrivalTime".Translate(lang), ValidationErrorType.InvalidDateRange);
            }

            if (unloadingDepartureTime.HasValue && unloadingArrivalTime.HasValue && unloadingArrivalTime > unloadingDepartureTime)
            {
                result.AddError(nameof(dto.UnloadingDepartureTime), "InvalidUnloadingDepartureTime".Translate(lang), ValidationErrorType.InvalidDateRange);
            }

            return result;
        }

        public override void MapFromDtoToEntity(Order entity, OrderDto dto)
        {
            bool isNew = string.IsNullOrEmpty(dto.Id);
            bool isInjection = dto.AdditionalInfo?.Contains("INJECTION") ?? false;

            _mapper.Map(dto, entity);

            if (isNew)
            {
                InitializeNewOrder(entity, isInjection);
                _historyService.Save(entity.Id, "orderSetDraft", entity.OrderNumber);
            }

            if (isInjection)
            {
                var file = dto.AdditionalInfo.Split(" - ").ElementAtOrDefault(1);
                _historyService.Save(entity.Id, isNew ? "orderCreatedFromInjection" : "orderUpdatedFromInjection", dto.OrderNumber, file);

                if (string.IsNullOrEmpty(entity.Source))
                {
                    entity.Source = file;
                }
                else
                {
                    entity.Source = $"{entity.Source};{file}";
                }
            }
        }

        protected override IEnumerable<OrderDto> FillLookupNames(IEnumerable<OrderDto> dtos)
        {
            var carrierIds = dtos.Where(x => !string.IsNullOrEmpty(x.CarrierId?.Value))
                                 .Select(x => x.CarrierId.Value.ToGuid())
                                 .ToList();
            var carriers = _dataService.GetDbSet<TransportCompany>()
                                       .Where(x => carrierIds.Contains(x.Id))
                                       .ToDictionary(x => x.Id.ToString());

            var pickingTypeIds = dtos.Where(x => !string.IsNullOrEmpty(x.PickingTypeId?.Value))
                                     .Select(x => x.PickingTypeId.Value.ToGuid())
                                     .ToList();
            var pickingTypes = _dataService.GetDbSet<PickingType>()
                                           .Where(x => pickingTypeIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            var shippingWarehouseIds = dtos.Where(x => !string.IsNullOrEmpty(x.ShippingWarehouseId?.Value))
                                           .Select(x => x.ShippingWarehouseId.Value.ToGuid())
                                           .ToList();
            var shippingWarehouses = _dataService.GetDbSet<ShippingWarehouse>()
                                                 .Where(x => shippingWarehouseIds.Contains(x.Id))
                                                 .ToDictionary(x => x.Id.ToString());
            
            var vehicleTypeIds = dtos.Where(x => !string.IsNullOrEmpty(x.VehicleTypeId?.Value))
                .Select(x => x.VehicleTypeId.Value.ToGuid())
                .ToList();
            
            var vehicleTypes = _dataService.GetDbSet<VehicleType>()
                .Where(x => vehicleTypeIds.Contains(x.Id))
                .ToDictionary(x => x.Id.ToString());
            

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.CarrierId?.Value)
                    && carriers.TryGetValue(dto.CarrierId.Value, out TransportCompany carrier))
                {
                    dto.CarrierId.Name = carrier.Title;
                }

                if (!string.IsNullOrEmpty(dto.PickingTypeId?.Value)
                    && pickingTypes.TryGetValue(dto.PickingTypeId.Value, out PickingType pickingType))
                {
                    dto.PickingTypeId.Name = pickingType.Name;
                }

                if (!string.IsNullOrEmpty(dto.ShippingWarehouseId?.Value)
                    && shippingWarehouses.TryGetValue(dto.ShippingWarehouseId.Value, out ShippingWarehouse shippingWarehouse))
                {
                    dto.ShippingWarehouseId.Name = shippingWarehouse.WarehouseName;
                }

                if (!string.IsNullOrEmpty(dto.VehicleTypeId?.Value)
                    && vehicleTypes.TryGetValue(dto.VehicleTypeId.Value, out VehicleType vehicleType))
                {
                    dto.VehicleTypeId.Name = vehicleType.Name;
                }
                
                
                yield return dto;
            }
        }

        public override void MapFromFormDtoToEntity(Order entity, OrderFormDto dto)
        {
            dto.ArticlesCount = (dto.Items?.Count).GetValueOrDefault();

            MapFromDtoToEntity(entity, dto);

            SaveItems(entity, dto);
        }

        public override OrderDto MapFromEntityToDto(Order entity)
        {
            if (entity == null)
            {
                return null;
            }

            var dto = _mapper.Map<OrderDto>(entity);

            return dto;
        }

        public override OrderFormDto MapFromEntityToFormDto(Order entity)
        {
            if (entity == null)
            {
                return null;
            }

            OrderDto dto = _mapper.Map<OrderDto>(entity);
            OrderFormDto result = _mapper.Map<OrderFormDto>(dto);

            result.Items = _dataService.GetDbSet<OrderItem>()
                                      .Where(i => i.OrderId == entity.Id)
                                      .Select(_mapper.Map<OrderItemDto>)
                                      .ToList();

            return result;
        }

        public override LookUpDto MapFromEntityToLookupDto(Order entity)
        {
            return new LookUpDto
            {
                Value = entity.Id.ToString(),
                Name = entity.OrderNumber
            };
        }

        private void InitializeNewOrder(Order order, bool isInjection)
        {
            order.IsActive = true;
            order.Status = OrderState.Draft;
            order.OrderCreationDate = DateTime.UtcNow;
            order.OrderChangeDate = DateTime.UtcNow;
            order.ShippingStatus = VehicleState.VehicleEmpty;
            order.DeliveryStatus = VehicleState.VehicleEmpty;
        }

        private void SaveItems(Order entity, OrderFormDto dto)
        {
            bool isManual = !(dto.AdditionalInfo?.Contains("INJECTION") ?? false);
            if (dto.Items != null)
            {
                HashSet<Guid> updatedItems = new HashSet<Guid>();
                List<OrderItem> entityItems = _dataService.GetDbSet<OrderItem>().Where(i => i.OrderId == entity.Id).ToList();
                Dictionary<string, OrderItem> entityItemsDict = entityItems.ToDictionary(i => i.Id.ToString());
                foreach (OrderItemDto itemDto in dto.Items)
                {
                    OrderItem item;
                    if (string.IsNullOrEmpty(itemDto.Id) || !entityItemsDict.TryGetValue(itemDto.Id, out item))
                    {
                        item = new OrderItem
                        {
                            OrderId = entity.Id
                        };
                        MapFromItemDtoToEntity(item, itemDto, true, isManual);

                        _historyService.Save(entity.Id, "orderItemAdded", item.Nart, item.Quantity);
                    }
                    else
                    {
                        updatedItems.Add(item.Id);
                        MapFromItemDtoToEntity(item, itemDto, false, isManual);
                        //_dataService.GetDbSet<OrderItem>().Update(item);
                    }
                }

                var itemsToRemove = entityItems.Where(i => !updatedItems.Contains(i.Id) && (isManual || !i.IsManualEdited));
                foreach (var item in itemsToRemove)
                {
                    _historyService.Save(entity.Id, "orderItemRemoved", item.Nart, item.Quantity);
                }
                _dataService.GetDbSet<OrderItem>().RemoveRange(itemsToRemove);

                entity.ArticlesCount = dto.Items.Count;
            }
        }

        private void MapFromItemDtoToEntity(OrderItem entity, OrderItemDto dto, bool isNew, bool isManual)
        {
            var setter = _fieldSetterFactory.Create<OrderItem>()
                .AddHandler(x => x.Nart, new OrderItemNartHandler(_dataService, _historyService, isNew))
                .AddHandler(x => x.Quantity, new OrderItemQuantityHandler(_historyService, isNew));

            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = dto.Id.ToGuid().Value;

            entity.Nart = dto.Nart;
            entity.Quantity = dto.Quantity ?? 0;

            if (isNew)
            {
                _dataService.GetDbSet<OrderItem>().Add(entity);
            }

            var change = _dataService.GetChanges<OrderItem>().FirstOrDefault(x => x.Entity.Id == entity.Id);
            setter.Appy(change);

            if (isManual && change.FieldChanges.Any())
            {
                entity.IsManualEdited = true;
            }
        }

        private readonly IMapper _mapper;

        /// <summary>
        /// Apply search form filter to query
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="searchForm">search form</param>
        /// <returns></returns>
        public override IQueryable<Order> ApplySearchForm(IQueryable<Order> query, FilterFormDto<OrderFilterDto> searchForm, List<string> columns = null)
        {
            List<object> parameters = new List<object>();
            string where = string.Empty;

            // OrderNumber Filter
            where = where.WhereAnd(searchForm.Filter.ActualPalletsCount.ApplyNumericFilter<Order>(i => i.ActualPalletsCount, ref parameters))
                         .WhereAnd(searchForm.Filter.ActualReturnDate.ApplyDateRangeFilter<Order>(i => i.ActualReturnDate, ref parameters))
                         .WhereAnd(searchForm.Filter.ActualWeightKg.ApplyNumericFilter<Order>(i => i.ActualWeightKg, ref parameters))
                         .WhereAnd(searchForm.Filter.ArticlesCount.ApplyNumericFilter<Order>(i => i.ArticlesCount, ref parameters))
                         .WhereAnd(searchForm.Filter.ClientOrderNumber.ApplyStringFilter<Order>(i => i.ClientOrderNumber, ref parameters))
                         .WhereAnd(searchForm.Filter.BoxesCount.ApplyNumericFilter<Order>(i => i.BoxesCount, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingAvisationTime.ApplyTimeRangeFilter<Order>(i => i.ShippingAvisationTime, ref parameters))
                         .WhereAnd(searchForm.Filter.ClientAvisationTime.ApplyTimeRangeFilter<Order>(i => i.ClientAvisationTime, ref parameters))
                         .WhereAnd(searchForm.Filter.ClientName.ApplyOptionsFilter<Order, string>(i => i.ClientName, ref parameters))
                         .WhereAnd(searchForm.Filter.ConfirmedBoxesCount.ApplyNumericFilter<Order>(i => i.ConfirmedBoxesCount, ref parameters))
                         .WhereAnd(searchForm.Filter.DeliveryAddress.ApplyStringFilter<Order>(i => i.DeliveryAddress, ref parameters))
                         .WhereAnd(searchForm.Filter.DeliveryCity.ApplyStringFilter<Order>(i => i.DeliveryCity, ref parameters))
                         .WhereAnd(searchForm.Filter.DeliveryDate.ApplyDateRangeFilter<Order>(i => i.DeliveryDate, ref parameters))
                         .WhereAnd(searchForm.Filter.DeliveryRegion.ApplyStringFilter<Order>(i => i.DeliveryRegion, ref parameters))
                         .WhereAnd(searchForm.Filter.DeliveryStatus.ApplyEnumFilter<Order, VehicleState>(i => i.DeliveryStatus, ref parameters))
                         .WhereAnd(searchForm.Filter.Invoice.ApplyBooleanFilter<Order>(i => i.Invoice, ref parameters))
                         .WhereAnd(searchForm.Filter.LoadingArrivalTime.ApplyDateRangeFilter<Order>(i => i.LoadingArrivalTime, ref parameters))
                         .WhereAnd(searchForm.Filter.LoadingDepartureTime.ApplyDateRangeFilter<Order>(i => i.LoadingDepartureTime, ref parameters))
                         .WhereAnd(searchForm.Filter.MajorAdoptionNumber.ApplyStringFilter<Order>(i => i.MajorAdoptionNumber, ref parameters))
                         .WhereAnd(searchForm.Filter.OrderAmountExcludingVAT.ApplyNumericFilter<Order>(i => i.OrderAmountExcludingVAT, ref parameters))
                         .WhereAnd(searchForm.Filter.OrderChangeDate.ApplyDateRangeFilter<Order>(i => i.OrderChangeDate, ref parameters))
                         .WhereAnd(searchForm.Filter.OrderComments.ApplyStringFilter<Order>(i => i.OrderComments, ref parameters))
                         .WhereAnd(searchForm.Filter.OrderCreationDate.ApplyDateRangeFilter<Order>(i => i.OrderCreationDate, ref parameters))
                         .WhereAnd(searchForm.Filter.OrderDate.ApplyDateRangeFilter<Order>(i => i.OrderDate, ref parameters))
                         .WhereAnd(searchForm.Filter.OrderNumber.ApplyStringFilter<Order>(i => i.OrderNumber, ref parameters))
                         .WhereAnd(searchForm.Filter.OrderShippingStatus.ApplyEnumFilter<Order, ShippingState>(i => i.OrderShippingStatus, ref parameters))
                         .WhereAnd(searchForm.Filter.OrderType.ApplyEnumFilter<Order, OrderType>(i => i.OrderType, ref parameters))
                         .WhereAnd(searchForm.Filter.PalletsCount.ApplyNumericFilter<Order>(i => i.PalletsCount, ref parameters))
                         .WhereAnd(searchForm.Filter.Payer.ApplyStringFilter<Order>(i => i.Payer, ref parameters))
                         .WhereAnd(searchForm.Filter.PickingTypeId.ApplyOptionsFilter<Order, Guid?>(i => i.PickingTypeId, ref parameters, i => new Guid(i)))
                         .WhereAnd(searchForm.Filter.PlannedReturnDate.ApplyDateRangeFilter<Order>(i => i.PlannedReturnDate, ref parameters))
                         .WhereAnd(searchForm.Filter.ReturnInformation.ApplyStringFilter<Order>(i => i.ReturnInformation, ref parameters))
                         .WhereAnd(searchForm.Filter.ReturnShippingAccountNo.ApplyStringFilter<Order>(i => i.ReturnShippingAccountNo, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingAddress.ApplyStringFilter<Order>(i => i.ShippingAddress, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingCity.ApplyStringFilter<Order>(i => i.ShippingCity, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingDate.ApplyDateRangeFilter<Order>(i => i.ShippingDate, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingId.ApplyOptionsFilter<Order, Guid?>(i => i.ShippingId, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingNumber.ApplyStringFilter<Order>(i => i.ShippingNumber, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingStatus.ApplyEnumFilter<Order, VehicleState>(i => i.ShippingStatus, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingWarehouseId.ApplyOptionsFilter<Order, Guid?>(i => i.ShippingWarehouseId, ref parameters, i => new Guid(i)))
                         .WhereAnd(searchForm.Filter.SoldTo.ApplyOptionsFilter<Order, string>(i => i.SoldTo, ref parameters))
                         .WhereAnd(searchForm.Filter.Status.ApplyEnumFilter<Order, OrderState>(i => i.Status, ref parameters))
                         .WhereAnd(searchForm.Filter.TemperatureMax.ApplyNumericFilter<Order>(i => i.TemperatureMax, ref parameters))
                         .WhereAnd(searchForm.Filter.TemperatureMin.ApplyNumericFilter<Order>(i => i.TemperatureMin, ref parameters))
                         .WhereAnd(searchForm.Filter.ConfirmedPalletsCount.ApplyNumericFilter<Order>(i => i.ConfirmedPalletsCount, ref parameters))
                         .WhereAnd(searchForm.Filter.TransitDays.ApplyNumericFilter<Order>(i => i.TransitDays, ref parameters))
                         .WhereAnd(searchForm.Filter.TrucksDowntime.ApplyNumericFilter<Order>(i => i.TrucksDowntime, ref parameters))
                         .WhereAnd(searchForm.Filter.UnloadingArrivalDate.ApplyDateRangeFilter<Order>(i => i.UnloadingArrivalTime, ref parameters))
                         .WhereAnd(searchForm.Filter.UnloadingArrivalTime.ApplyTimeRangeFilter<Order>(i => i.UnloadingArrivalTime, ref parameters))
                         .WhereAnd(searchForm.Filter.UnloadingDepartureDate.ApplyDateRangeFilter<Order>(i => i.UnloadingDepartureTime, ref parameters))
                         .WhereAnd(searchForm.Filter.UnloadingDepartureTime.ApplyTimeRangeFilter<Order>(i => i.UnloadingDepartureTime, ref parameters))
                         .WhereAnd(searchForm.Filter.DocumentsReturnDate.ApplyDateRangeFilter<Order>(i => i.DocumentsReturnDate, ref parameters))
                         .WhereAnd(searchForm.Filter.ActualDocumentsReturnDate.ApplyDateRangeFilter<Order>(i => i.ActualDocumentsReturnDate, ref parameters))
                         .WhereAnd(searchForm.Filter.WeightKg.ApplyNumericFilter<Order>(i => i.WeightKg, ref parameters))
                         .WhereAnd(searchForm.Filter.WaybillTorg12.ApplyBooleanFilter<Order>(i => i.WaybillTorg12, ref parameters))
                         .WhereAnd(searchForm.Filter.PickingFeatures.ApplyStringFilter<Order>(i => i.PickingFeatures, ref parameters))
                         .WhereAnd(searchForm.Filter.CarrierId.ApplyOptionsFilter<Order, Guid?>(i => i.CarrierId, ref parameters, i => new Guid(i)))
                         .WhereAnd(searchForm.Filter.DeliveryType.ApplyEnumFilter<Order, DeliveryType>(i => i.DeliveryType, ref parameters))
                         .WhereAnd(searchForm.Filter.DeviationsComment.ApplyStringFilter<Order>(i => i.DeviationsComment, ref parameters))
                         .WhereAnd(searchForm.Filter.DeliveryCost.ApplyNumericFilter<Order>(i => i.DeliveryCost, ref parameters))
                         .WhereAnd(searchForm.Filter.ActualDeliveryCost.ApplyNumericFilter<Order>(i => i.ActualDeliveryCost, ref parameters));

            string sql = $@"SELECT * FROM ""Orders"" {where}";
            query = query.FromSql(sql, parameters.ToArray());

            // Apply Search
            query = this.ApplySearch(query, searchForm?.Filter?.Search, columns ?? searchForm?.Filter?.Columns);

            var sortFieldMapping = new Dictionary<string, string>
            {
                { "unloadingArrivalDate", "unloadingArrivalTime" },
                { "unloadingDepartureDate", "unloadingDepartureTime" }
            };

            if (!string.IsNullOrEmpty(searchForm.Sort?.Name) && sortFieldMapping.ContainsKey(searchForm.Sort?.Name))
            {
                searchForm.Sort.Name = sortFieldMapping[searchForm.Sort?.Name];
            }

            return query.OrderBy(searchForm.Sort?.Name, searchForm.Sort?.Desc == true)
                .DefaultOrderBy(i => i.OrderCreationDate, !string.IsNullOrEmpty(searchForm.Sort?.Name))
                .DefaultOrderBy(i => i.Id, true);
        }

        private IQueryable<Order> ApplySearch(IQueryable<Order> query, string search, List<string> columns)
        {
            var searchDateFormat = "dd.MM.yyyy HH:mm";

            if (string.IsNullOrEmpty(search)) return query;

            var isInt = int.TryParse(search, out int searchInt);
            var isDecimal = decimal.TryParse(search, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal searchDecimal);
            decimal precision = 0.01M;

            var pickingTypes = _dataService.GetDbSet<PickingType>().Where(i => i.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase));

            var carriers = _dataService.GetDbSet<TransportCompany>().Where(i => i.Title.Contains(search, StringComparison.InvariantCultureIgnoreCase));

            var orderTypeNames = Enum.GetNames(typeof(OrderType)).Select(i => i.ToLower());

            var orderTypes = _dataService.GetDbSet<Translation>()
                .Where(i => orderTypeNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (OrderType?)Enum.Parse<OrderType>(i.Name, true))
                .ToList();

            var orderShippingStateNames = Enum.GetNames(typeof(ShippingState)).Select(i => i.ToLower());

            var orderShippingStates = _dataService.GetDbSet<Translation>()
                .Where(i => orderShippingStateNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (ShippingState?)Enum.Parse<ShippingState>(i.Name, true))
                .ToList();

            var orderStateNames = Enum.GetNames(typeof(OrderState)).Select(i => i.ToLower());

            var orderStates = _dataService.GetDbSet<Translation>()
                .Where(i => orderStateNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (OrderState?)Enum.Parse<OrderState>(i.Name, true))
                .ToList();

            var vehicleStateNames = Enum.GetNames(typeof(VehicleState)).Select(i => i.ToLower());

            var vehicleStates = _dataService.GetDbSet<Translation>()
                .Where(i => vehicleStateNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (VehicleState?)Enum.Parse<VehicleState>(i.Name, true))
                .ToList();

            var deliveryTypeNames = Enum.GetNames(typeof(DeliveryType)).Select(i => i.ToLower());

            var deliveryTypes = _dataService.GetDbSet<Translation>()
                .Where(i => deliveryTypeNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (DeliveryType?)Enum.Parse<DeliveryType>(i.Name, true))
                .ToList();

            return query.Where(i =>
                   columns.Contains("orderNumber") && !string.IsNullOrEmpty(i.OrderNumber) && i.OrderNumber.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("shippingNumber") && !string.IsNullOrEmpty(i.ShippingNumber) && i.ShippingNumber.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("clientName") && !string.IsNullOrEmpty(i.ClientName) && i.ClientName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("soldTo") && !string.IsNullOrEmpty(i.SoldTo) && i.SoldTo.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("orderDate") && i.OrderDate.HasValue && i.OrderDate.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("payer") && !string.IsNullOrEmpty(i.Payer) && i.Payer.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("temperatureMin") && isInt && i.TemperatureMin == searchInt
                || columns.Contains("temperatureMax") && isInt && i.TemperatureMax == searchInt
                || columns.Contains("shippingDate") && i.ShippingDate.HasValue && i.ShippingDate.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("shippingAddress") && !string.IsNullOrEmpty(i.ShippingAddress) && i.ShippingAddress.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("shippingCity") && !string.IsNullOrEmpty(i.ShippingCity) && i.ShippingCity.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("transitDays") && isInt && i.TransitDays == searchInt
                || columns.Contains("deliveryRegion") && !string.IsNullOrEmpty(i.DeliveryRegion) && i.DeliveryRegion.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("deliveryCity") && !string.IsNullOrEmpty(i.DeliveryCity) && i.DeliveryCity.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("deliveryAddress") && !string.IsNullOrEmpty(i.DeliveryAddress) && i.DeliveryAddress.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("deliveryDate") && i.DeliveryDate.HasValue && i.DeliveryDate.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("articlesCount") && isInt && i.ArticlesCount == searchInt
                || columns.Contains("boxesCount") && isDecimal && i.BoxesCount >= searchDecimal - precision && i.BoxesCount <= searchDecimal + precision
                || columns.Contains("confirmedBoxesCount") && isDecimal && i.ConfirmedBoxesCount >= searchDecimal - precision && i.ConfirmedBoxesCount <= searchDecimal + precision
                || columns.Contains("confirmedPalletsCount") && isDecimal && i.ConfirmedPalletsCount >= searchDecimal - precision && i.ConfirmedPalletsCount <= searchDecimal + precision
                || columns.Contains("palletsCount") && isInt && i.PalletsCount == searchInt
                || columns.Contains("actualPalletsCount") && isInt && i.ActualPalletsCount == searchInt
                || columns.Contains("weightKg") && isDecimal && i.WeightKg >= searchDecimal - precision && i.WeightKg <= searchDecimal + precision
                || columns.Contains("actualWeightKg") && isDecimal && i.ActualWeightKg >= searchDecimal - precision && i.ActualWeightKg <= searchDecimal + precision
                || columns.Contains("orderAmountExcludingVAT") && isDecimal && i.OrderAmountExcludingVAT >= searchDecimal - precision && i.OrderAmountExcludingVAT <= searchDecimal + precision
                || columns.Contains("clientOrderNumber") && !string.IsNullOrEmpty(i.ClientOrderNumber) && i.ClientOrderNumber.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("loadingArrivalTime") && i.LoadingArrivalTime.HasValue && i.LoadingArrivalTime.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("loadingDepartureTime") && i.LoadingDepartureTime.HasValue && i.LoadingDepartureTime.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("unloadingArrivalTime") && i.UnloadingArrivalTime.HasValue && i.UnloadingArrivalTime.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("unloadingDepartureTime") && i.UnloadingDepartureTime.HasValue && i.UnloadingDepartureTime.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("trucksDowntime") && isDecimal && i.TrucksDowntime >= searchDecimal - precision && i.TrucksDowntime <= searchDecimal + precision
                || columns.Contains("returnInformation") && !string.IsNullOrEmpty(i.ReturnInformation) && i.ReturnInformation.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("returnShippingAccountNo") && !string.IsNullOrEmpty(i.ReturnShippingAccountNo) && i.ReturnShippingAccountNo.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("plannedReturnDate") && i.PlannedReturnDate.HasValue && i.PlannedReturnDate.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("actualReturnDate") && i.ActualReturnDate.HasValue && i.ActualReturnDate.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("actualDocumentsReturnDate") && i.ActualDocumentsReturnDate.HasValue && i.ActualDocumentsReturnDate.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("majorAdoptionNumber") && !string.IsNullOrEmpty(i.MajorAdoptionNumber) && i.MajorAdoptionNumber.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("orderComments") && !string.IsNullOrEmpty(i.OrderComments) && i.OrderComments.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("shippingAvisationTime") && i.ShippingAvisationTime.HasValue && i.ShippingAvisationTime.ToString().Contains(search)
                || columns.Contains("clientAvisationTime") && i.ClientAvisationTime.HasValue && i.ClientAvisationTime.ToString().Contains(search)
                || columns.Contains("orderCreationDate") && i.OrderCreationDate.HasValue && i.OrderCreationDate.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("orderChangeDate") && i.OrderChangeDate.HasValue && i.OrderChangeDate.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("deviationsComment") && !string.IsNullOrEmpty(i.DeviationsComment) && i.DeviationsComment.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || columns.Contains("deliveryCost") && isDecimal && i.DeliveryCost >= searchDecimal - precision && i.DeliveryCost <= searchDecimal + precision
                || columns.Contains("actualDeliveryCost") && isDecimal && i.ActualDeliveryCost >= searchDecimal - precision && i.ActualDeliveryCost <= searchDecimal + precision

                || columns.Contains("pickingTypeId") && pickingTypes.Any(p => p.Id == i.PickingTypeId)
                || columns.Contains("carrierId") && carriers.Any(p => p.Id == i.CarrierId)
                || columns.Contains("orderType") && orderTypes.Contains(i.OrderType)
                || columns.Contains("orderShippingStatus") && orderShippingStates.Contains(i.OrderShippingStatus)
                || columns.Contains("deliveryStatus") && vehicleStates.Contains(i.DeliveryStatus)
                || columns.Contains("shippingStatus") && vehicleStates.Contains(i.ShippingStatus)
                || columns.Contains("status") && orderStates.Contains(i.Status)
                || columns.Contains("deliveryType") && deliveryTypes.Contains(i.DeliveryType)
                );
        }

        protected override ExcelMapper<OrderDto> CreateExportExcelMapper()
        {
            string lang = _userIdProvider.GetCurrentUser()?.Language;
            return base.CreateExportExcelMapper()
                .MapColumn(i => i.PickingTypeId, new DictionaryReferenceExcelColumn(GetPickingTypeIdByName))
                .MapColumn(i => i.ShippingWarehouseId, new DictionaryReferenceExcelColumn(GetShippingWarehouseIdByName))
                .MapColumn(i => i.CarrierId, new DictionaryReferenceExcelColumn(GetCarrierIdByName))
                .MapColumn(i => i.Status, new StateExcelColumn<OrderState>(lang))
                .MapColumn(i => i.OrderShippingStatus, new StateExcelColumn<ShippingState>(lang))
                .MapColumn(i => i.ShippingStatus, new StateExcelColumn<VehicleState>(lang))
                .MapColumn(i => i.DeliveryStatus, new StateExcelColumn<VehicleState>(lang))
                .MapColumn(i => i.OrderType, new EnumExcelColumn<OrderType>(lang))
                .MapColumn(i => i.DeliveryType, new EnumExcelColumn<DeliveryType>(lang))
                .MapColumn(i => i.TarifficationType, new EnumExcelColumn<TarifficationType>(lang))
                .MapColumn(w => w.VehicleTypeId, new DictionaryReferenceExcelColumn(GetVehicleTypeIdByName))
                ;
        }

        private Guid? GetPickingTypeIdByName(string name)
        {
            var entry = _dataService.GetDbSet<PickingType>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        private Guid? GetShippingWarehouseIdByName(string name)
        {
            var entry = _dataService.GetDbSet<ShippingWarehouse>().Where(t => t.WarehouseName == name).FirstOrDefault();
            return entry?.Id;
        }

        private Guid? GetCarrierIdByName(string name)
        {
            var entry = _dataService.GetDbSet<TransportCompany>().Where(t => t.Title == name).FirstOrDefault();
            return entry?.Id;
        }

        private string GetCarrierNameById(Guid? id)
        {
            return id == null ? null : _dataService.GetById<TransportCompany>(id.Value)?.Title;
        }
        
        private Guid? GetVehicleTypeIdByName(string name)
        {
            var entry = _dataService.GetDbSet<VehicleType>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }        
    }
}
using Application.BusinessModels.Orders.Handlers;
using Application.BusinessModels.Shared.Actions;
using Application.Extensions;
using Application.Shared;
using AutoMapper;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
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
using Domain.Services.Translations;

namespace Application.Services.Orders
{
    public class OrdersService : GridService<Order, OrderDto, OrderFormDto, OrderSummaryDto, OrderFilterDto>, IOrdersService
    {
        private readonly IHistoryService _historyService;
        private readonly IFieldPropertiesService _fieldPropertiesService;

        public OrdersService(
            IHistoryService historyService,
            ICommonDataService dataService,
            IUserProvider userIdProvider,
            IFieldDispatcherService fieldDispatcherService,
            IFieldPropertiesService fieldPropertiesService,
            IEnumerable<IAppAction<Order>> singleActions,
            IEnumerable<IGroupAppAction<Order>> groupActions)
            : base(dataService, userIdProvider, fieldDispatcherService, fieldPropertiesService, singleActions, groupActions)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
            _fieldPropertiesService = fieldPropertiesService;
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

        public override ValidateResult MapFromDtoToEntity(Order entity, OrderDto dto)
        {
            var validateResult = ValidateDto(dto);
            if (validateResult.IsError)
            {
                return validateResult;
            }
            
            bool isNew = string.IsNullOrEmpty(dto.Id);
            bool isInjection = dto.AdditionalInfo?.Contains("INJECTION") ?? false;

            IEnumerable<string> readOnlyFields = null;
            if (!isNew)
            {
                var userId = _userIdProvider.GetCurrentUserId();
                if (userId != null)
                {
                    string stateName = entity.Status.ToString().ToLowerFirstLetter();
                    readOnlyFields = _fieldPropertiesService.GetReadOnlyFields(FieldPropertiesForEntityType.Orders, stateName, null, null, userId);
                }
            }

            var setter = new FieldSetter<Order>(entity, _historyService, readOnlyFields);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id), ignoreChanges: true);
            if (!string.IsNullOrEmpty(dto.ShippingWarehouseId))
                setter.UpdateField(e => e.ShippingWarehouseId, Guid.Parse(dto.ShippingWarehouseId), new ShippingWarehouseHandler(_dataService, _historyService), nameLoader: GetShippingWarehouseNameById);
            if (!string.IsNullOrEmpty(dto.Status))
                setter.UpdateField(e => e.Status, MapFromStateDto<OrderState>(dto.Status), ignoreChanges: true);
            if (!string.IsNullOrEmpty(dto.ShippingStatus))
                setter.UpdateField(e => e.ShippingStatus, MapFromStateDto<VehicleState>(dto.ShippingStatus), new ShippingStatusHandler(_dataService, _historyService));
            if (!string.IsNullOrEmpty(dto.DeliveryStatus))
                setter.UpdateField(e => e.DeliveryStatus, MapFromStateDto<VehicleState>(dto.DeliveryStatus), new DeliveryStatusHandler(_dataService, _historyService));
            setter.UpdateField(e => e.ClientOrderNumber, dto.ClientOrderNumber, new ClientOrderNumberHandler(_historyService));
            setter.UpdateField(e => e.OrderDate, ParseDateTime(dto.OrderDate));
            setter.UpdateField(e => e.OrderType, string.IsNullOrEmpty(dto.OrderType) ? (OrderType?)null : MapFromStateDto<OrderType>(dto.OrderType));
            setter.UpdateField(e => e.Payer, dto.Payer);
            setter.UpdateField(e => e.ClientName, dto.ClientName);
            setter.UpdateField(e => e.SoldTo, dto.SoldTo, new SoldToHandler(_dataService, _historyService));
            setter.UpdateField(e => e.TemperatureMin, dto.TemperatureMin);
            setter.UpdateField(e => e.TemperatureMax, dto.TemperatureMax);
            setter.UpdateField(e => e.ShippingDate, ParseDateTime(dto.ShippingDate), new ShippingDateHandler(_dataService, _historyService, !isInjection));
            setter.UpdateField(e => e.TransitDays, dto.TransitDays);
            setter.UpdateField(e => e.DeliveryDate, ParseDateTime(dto.DeliveryDate), new DeliveryDateHandler(_dataService, _historyService, isInjection));
            setter.UpdateField(e => e.OrderNumber, dto.OrderNumber, new OrderNumberHandler(_userIdProvider, _dataService));
            setter.UpdateField(e => e.ArticlesCount, dto.ArticlesCount, new ArticlesCountHandler());
            setter.UpdateField(e => e.BoxesCount, Round(dto.BoxesCount, 1), new BoxesCountHandler());
            setter.UpdateField(e => e.ConfirmedBoxesCount, Round(dto.ConfirmedBoxesCount, 1), new ConfirmedBoxesCountHandler());
            setter.UpdateField(e => e.PalletsCount, dto.PalletsCount, new PalletsCountHandler(_dataService, _historyService, !isInjection));
            setter.UpdateField(e => e.ConfirmedPalletsCount, dto.ConfirmedPalletsCount, new ConfirmedPalletsCountHandler(_dataService, _historyService));
            setter.UpdateField(e => e.ActualPalletsCount, dto.ActualPalletsCount, new ActualPalletsCountHandler(_dataService, _historyService));
            setter.UpdateField(e => e.WeightKg, dto.WeightKg, new WeightKgHandler(_dataService, _historyService));
            setter.UpdateField(e => e.ActualWeightKg, dto.ActualWeightKg, new ActualWeightKgHandler(_dataService, _historyService));
            setter.UpdateField(e => e.OrderAmountExcludingVAT, dto.OrderAmountExcludingVAT, new OrderAmountExcludingVATHandler());
            setter.UpdateField(e => e.InvoiceAmountExcludingVAT, dto.InvoiceAmountExcludingVAT);
            setter.UpdateField(e => e.DeliveryRegion, dto.DeliveryRegion);
            setter.UpdateField(e => e.DeliveryCity, dto.DeliveryCity);
            setter.UpdateField(e => e.ShippingAddress, dto.ShippingAddress);
            setter.UpdateField(e => e.DeliveryAddress, dto.DeliveryAddress);
            setter.UpdateField(e => e.ClientAvisationTime, ParseTime(dto.ClientAvisationTime), new ClientAvisationTimeHandler());
            setter.UpdateField(e => e.OrderComments, dto.OrderComments);
            setter.UpdateField(e => e.PickingTypeId, string.IsNullOrEmpty(dto.PickingTypeId) ? (Guid?)null : Guid.Parse(dto.PickingTypeId), new PickingTypeHandler(!isInjection), nameLoader: GetPickingTypeNameById);
            setter.UpdateField(e => e.PlannedArrivalTimeSlotBDFWarehouse, dto.PlannedArrivalTimeSlotBDFWarehouse);
            setter.UpdateField(e => e.LoadingArrivalTime, ParseDateTime(dto.LoadingArrivalTime), new LoadingArrivalTimeHandler(_dataService, _historyService));
            setter.UpdateField(e => e.LoadingDepartureTime, ParseDateTime(dto.LoadingDepartureTime), new LoadingDepartureTimeHandler(_dataService, _historyService));
            setter.UpdateField(e => e.UnloadingArrivalTime, ParseDateTime(dto.UnloadingArrivalDate)?.Add(ParseTime(dto.UnloadingArrivalTime) ?? TimeSpan.Zero), new UnloadingArrivalTimeHandler(_dataService, _historyService));
            setter.UpdateField(e => e.UnloadingDepartureTime, ParseDateTime(dto.UnloadingDepartureDate)?.Add(ParseTime(dto.UnloadingDepartureTime) ?? TimeSpan.Zero), new UnloadingDepartureTimeHandler(_dataService, _historyService));
            setter.UpdateField(e => e.DocumentsReturnDate, ParseDateTime(dto.DocumentsReturnDate));
            setter.UpdateField(e => e.ActualDocumentsReturnDate, ParseDateTime(dto.ActualDocumentsReturnDate));
            setter.UpdateField(e => e.WaybillTorg12, dto.WaybillTorg12 ?? false);
            setter.UpdateField(e => e.Invoice, dto.Invoice ?? false);
            setter.UpdateField(e => e.TrucksDowntime, dto.TrucksDowntime, new TrucksDowntimeHandler(_dataService, _historyService));
            setter.UpdateField(e => e.ReturnInformation, dto.ReturnInformation);
            setter.UpdateField(e => e.ReturnShippingAccountNo, dto.ReturnShippingAccountNo);
            setter.UpdateField(e => e.PlannedReturnDate, ParseDateTime(dto.PlannedReturnDate));
            setter.UpdateField(e => e.ActualReturnDate, ParseDateTime(dto.ActualReturnDate));
            setter.UpdateField(e => e.MajorAdoptionNumber, dto.MajorAdoptionNumber);
            setter.UpdateField(e => e.OrderConfirmed, dto.OrderConfirmed ?? false);
            setter.UpdateField(e => e.DocumentReturnStatus, dto.DocumentReturnStatus.GetValueOrDefault());
            setter.UpdateField(e => e.Source, dto.Source, ignoreChanges: true);

            /*end of map dto to entity fields*/

            if (isNew)
            {
                InitializeNewOrder(setter, isInjection);
            }

            setter.ApplyAfterActions();

            bool isCreated = false;
            if (setter.HasChanges)
            {
                isCreated = CheckRequiredFields(entity);
            }

            if (isNew && !isCreated)
            {
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

            setter.SaveHistoryLog();

            string errors = setter.ValidationErrors;
            return new ValidateResult(errors, entity.Id.ToString());
        }
        
        private ValidateResult ValidateDto(OrderDto dto)
        {
            var lang = _userIdProvider.GetCurrentUser()?.Language;

            DetailedValidattionResult result = new DetailedValidattionResult();

            if (string.IsNullOrEmpty(dto.OrderNumber))
                result.AddError(nameof(dto.OrderNumber), "empty{OrderNumber}".Translate(lang),
                    ValidationErrorType.ValueIsRequired);

            if (string.IsNullOrEmpty(dto.ClientOrderNumber))
                result.AddError(nameof(dto.ClientOrderNumber), "emptyClientOrderNumber".Translate(lang),
                    ValidationErrorType.ValueIsRequired);

            if (string.IsNullOrEmpty(dto.OrderDate))
                result.AddError(nameof(dto.OrderDate), "emptyOrderDate".Translate(lang),
                    ValidationErrorType.ValueIsRequired);


            if (string.IsNullOrEmpty(dto.SoldTo))
                result.AddError(nameof(dto.SoldTo), "emptySoldTo".Translate(lang), ValidationErrorType.ValueIsRequired);

            if (string.IsNullOrEmpty(dto.ShippingAddress))
                result.AddError(nameof(dto.ShippingAddress), "emptyShippingAddress".Translate(lang),
                    ValidationErrorType.ValueIsRequired);

            return result;
        }

        public override ValidateResult MapFromFormDtoToEntity(Order entity, OrderFormDto dto)
        {
            dto.ArticlesCount = (dto.Items?.Count).GetValueOrDefault();

            ValidateResult result = MapFromDtoToEntity(entity, dto);
            if (!result.IsError)
            {
                SaveItems(entity, dto);
            }
            return result;
        }

        public override OrderDto MapFromEntityToDto(Order entity)
        {
            if (entity == null)
            {
                return null;
            }

            var dto = _mapper.Map<OrderDto>(entity);
            var warehouse = _dataService.GetDbSet<Warehouse>()
                .Where(i => i.SoldToNumber == dto.SoldTo)
                .FirstOrDefault();

            dto.PickingFeatures = warehouse?.PickingFeatures;

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

        private string GetPickingTypeNameById(Guid? id)
        {
            return id == null ? null : _dataService.GetById<PickingType>(id.Value)?.Name;
        }

        private string GetShippingWarehouseNameById(Guid? id)
        {
            return id == null ? null : _dataService.GetById<ShippingWarehouse>(id.Value)?.WarehouseName;
        }

        private bool CheckRequiredFields(Order order)
        {
            if (order.Status == OrderState.Draft)
            {
                bool hasRequiredFields =
                       !string.IsNullOrEmpty(order.SoldTo)
                    && !string.IsNullOrEmpty(order.ShippingAddress)
                    && !string.IsNullOrEmpty(order.DeliveryAddress)
                    && !string.IsNullOrEmpty(order.Payer)
                    && order.ShippingWarehouseId.HasValue
                    && order.DeliveryWarehouseId.HasValue
                    && order.ShippingDate.HasValue
                    && order.DeliveryDate.HasValue;

                if (hasRequiredFields)
                {
                    order.Status = OrderState.Created;
                    _historyService.Save(order.Id, "orderSetCreated", order.OrderNumber);
                    return true;
                }
            }
            return false;
        }

        private void InitializeNewOrder(FieldSetter<Order> setter, bool isInjection)
        {
            if (!isInjection && string.IsNullOrEmpty(setter.Entity.ShippingAddress))
            {
                var fromWarehouse = _dataService.GetDbSet<Warehouse>().FirstOrDefault(x => !x.CustomerWarehouse);
                if (fromWarehouse != null)
                {
                    setter.UpdateField(e => e.ShippingWarehouseId, fromWarehouse.Id, ignoreChanges: true);
                    setter.UpdateField(e => e.ShippingAddress, fromWarehouse.Address);
                }
            }

            setter.UpdateField(e => e.IsActive, true, ignoreChanges: true);
            setter.UpdateField(e => e.Status, OrderState.Draft, ignoreChanges: true);
            setter.UpdateField(e => e.OrderCreationDate, DateTime.Now, ignoreChanges: true);
            setter.UpdateField(e => e.OrderChangeDate, DateTime.Now, ignoreChanges: true);
            setter.UpdateField(e => e.ShippingStatus, VehicleState.VehicleEmpty);
            setter.UpdateField(e => e.DeliveryStatus, VehicleState.VehicleEmpty);
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
                        _dataService.GetDbSet<OrderItem>().Add(item);

                        _historyService.Save(entity.Id, "orderItemAdded", item.Nart, item.Quantity);
                    }
                    else
                    {
                        updatedItems.Add(item.Id);
                        MapFromItemDtoToEntity(item, itemDto, false, isManual);
                        _dataService.GetDbSet<OrderItem>().Update(item);
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
            var setter = new FieldSetter<OrderItem>(entity, _historyService);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id), ignoreChanges: true);
            setter.UpdateField(x => x.Nart, dto.Nart, new OrderItemNartHandler(_dataService, _historyService, isNew));
            setter.UpdateField(x => x.Quantity, dto.Quantity ?? 0, new OrderItemQuantityHandler(_historyService, isNew));

            setter.ApplyAfterActions();

            if (isManual && setter.HasChanges)
            {
                entity.IsManualEdited = true;
            }
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrderDto, OrderFormDto>();

                cfg.CreateMap<Order, OrderDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.Status, e => e.MapFrom((s, t) => s.Status.ToString().ToLowerFirstLetter()))
                    .ForMember(t => t.OrderType, e => e.MapFrom((s, t) => s.OrderType?.ToString()?.ToLowerFirstLetter()))
                    .ForMember(t => t.OrderDate, e => e.MapFrom((s, t) => s.OrderDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ShippingStatus, e => e.MapFrom((s, t) => s.ShippingStatus.ToString().ToLowerFirstLetter()))
                    .ForMember(t => t.DeliveryStatus, e => e.MapFrom((s, t) => s.DeliveryStatus.ToString().ToLowerFirstLetter()))
                    .ForMember(t => t.OrderShippingStatus, e => e.MapFrom((s, t) => s.OrderShippingStatus?.ToString()?.ToLowerFirstLetter()))
                    .ForMember(t => t.PickingTypeId, e => e.MapFrom((s, t) => s.PickingTypeId?.ToString()))
                    .ForMember(t => t.ShippingWarehouseId, e => e.MapFrom((s, t) => s.ShippingWarehouseId?.ToString()))
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
                    .ForMember(t => t.ClientAvisationTime, e => e.MapFrom((s, t) => s.ClientAvisationTime?.ToString(@"hh\:mm")))
                    .ForMember(t => t.OrderCreationDate, e => e.MapFrom((s, t) => s.OrderCreationDate?.ToString("dd.MM.yyyy HH:mm")))
                    .ForMember(t => t.OrderChangeDate, e => e.MapFrom((s, t) => s.OrderChangeDate?.ToString("dd.MM.yyyy HH:mm")));

                cfg.CreateMap<OrderItem, OrderItemDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()));
            });
            return result;
        }

        private readonly IMapper _mapper;

        /// <summary>
        /// Apply search form filter to query
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="searchForm">search form</param>
        /// <returns></returns>
        public override IQueryable<Order> ApplySearchForm(IQueryable<Order> query, FilterFormDto<OrderFilterDto> searchForm)
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
                         .WhereAnd(searchForm.Filter.ClientAvisationTime.ApplyTimeRangeFilter<Order>(i => i.ClientAvisationTime, ref parameters))
                         .WhereAnd(searchForm.Filter.ClientName.ApplyStringFilter<Order>(i => i.ClientName, ref parameters))
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
                         .WhereAnd(searchForm.Filter.ShippingDate.ApplyDateRangeFilter<Order>(i => i.ShippingDate, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingId.ApplyOptionsFilter<Order, Guid?>(i => i.ShippingId, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingNumber.ApplyStringFilter<Order>(i => i.ShippingNumber, ref parameters))
                         .WhereAnd(searchForm.Filter.ShippingStatus.ApplyEnumFilter<Order, VehicleState>(i => i.ShippingStatus, ref parameters))
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
                         .WhereAnd(searchForm.Filter.WaybillTorg12.ApplyBooleanFilter<Order>(i => i.WaybillTorg12, ref parameters));

            string sql = $@"SELECT * FROM ""Orders"" {where}";
            query = query.FromSql(sql, parameters.ToArray());

            // Apply Search
            query = this.ApplySearch(query, searchForm);

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

        private IQueryable<Order> ApplySearch(IQueryable<Order> query, FilterFormDto<OrderFilterDto> searchForm)
        {
            var search = searchForm.Filter.Search;
            var columns = searchForm.Filter.Columns;

            var searchDateFormat = "dd.MM.yyyy HH:mm";

            if (string.IsNullOrEmpty(search)) return query;

            var isInt = int.TryParse(search, out int searchInt);
            var isDecimal = decimal.TryParse(search, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal searchDecimal);
            decimal precision = 0.01M;

            var pickingTypes = this._dataService.GetDbSet<PickingType>().Where(i => i.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase));
            
            var orderTypeNames = Enum.GetNames(typeof(OrderType)).Select(i => i.ToLower());

            var orderTypes = this._dataService.GetDbSet<Translation>()
                .Where(i => orderTypeNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (OrderType?)Enum.Parse<OrderType>(i.Name, true))
                .ToList();

            var orderShippingStateNames = Enum.GetNames(typeof(ShippingState)).Select(i => i.ToLower());

            var orderShippingStates = this._dataService.GetDbSet<Translation>()
                .Where(i => orderShippingStateNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (ShippingState?)Enum.Parse<ShippingState>(i.Name, true))
                .ToList();

            var orderStateNames = Enum.GetNames(typeof(OrderState)).Select(i => i.ToLower());

            var orderStates = this._dataService.GetDbSet<Translation>()
                .Where(i => orderStateNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (OrderState?)Enum.Parse<OrderState>(i.Name, true))
                .ToList();

            var vehicleStateNames = Enum.GetNames(typeof(VehicleState)).Select(i => i.ToLower());

            var vehicleStates = this._dataService.GetDbSet<Translation>()
                .Where(i => vehicleStateNames.Contains(i.Name.ToLower()))
                .WhereTranslation(search)
                .Select(i => (VehicleState?)Enum.Parse<VehicleState>(i.Name, true))
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
                || columns.Contains("clientAvisationTime") && i.ClientAvisationTime.HasValue && i.ClientAvisationTime.ToString().Contains(search)
                || columns.Contains("orderCreationDate") && i.OrderCreationDate.HasValue && i.OrderCreationDate.Value.ToString(searchDateFormat).Contains(search)
                || columns.Contains("orderChangeDate") && i.OrderChangeDate.HasValue && i.OrderChangeDate.Value.ToString(searchDateFormat).Contains(search)

                || columns.Contains("pickingTypeId") && pickingTypes.Any(p => p.Id == i.PickingTypeId)
                || columns.Contains("orderType") && orderTypes.Contains(i.OrderType)
                || columns.Contains("orderShippingStatus") && orderShippingStates.Contains(i.OrderShippingStatus)
                || columns.Contains("deliveryStatus") && vehicleStates.Contains(i.DeliveryStatus)
                || columns.Contains("shippingStatus") && vehicleStates.Contains(i.ShippingStatus)
                || columns.Contains("status") && orderStates.Contains(i.Status)
                );
        }
    }
}
using Application.BusinessModels.Orders.Handlers;
using Application.Extensions;
using Application.Shared;
using AutoMapper;
using DAL;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Orders;
using Domain.Services.UserProvider;
using Domain.Shared;
using Domain.Shared.FormFilters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Application.Services.Orders
{
    public class OrdersService : GridService<Order, OrderDto, OrderFormDto, OrderSummaryDto, FilterFormDto<OrderFilterDto>>, IOrdersService
    {
        private readonly IHistoryService _historyService;

        public OrdersService(ICommonDataService dataSevice, AppDbContext db, IUserProvider userIdProvider, IHistoryService historyService, IActionService<Order> actionService) 
            : base(dataSevice, db, userIdProvider, actionService)
        {
            _mapper = ConfigureMapper().CreateMapper();
            this._historyService = historyService;
        }

        public override OrderSummaryDto GetSummary(IEnumerable<Guid> ids)
        {
            var orders = this.dataService.GetDbSet<Order>().Where(o => ids.Contains(o.Id)).ToList();
            var result = new OrderSummaryDto
            {
                Count = orders.Count,
                BoxesCount = orders.Sum(o => o.BoxesCount ?? 0),
                PalletsCount = orders.Sum(o => o.PalletsCount ?? 0),
                WeightKg = orders.Sum(o => o.WeightKg ?? 0M)
            };
            return result;
        }

        public override ValidateResult MapFromDtoToEntity(Order entity, OrderDto dto)
        {
            var setter = new FieldSetter<Order>(entity, _historyService);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id), ignoreChanges: true);
            if (!string.IsNullOrEmpty(dto.Status))
                setter.UpdateField(e => e.Status, MapFromStateDto<OrderState>(dto.Status), ignoreChanges: true);
            if (!string.IsNullOrEmpty(dto.ShippingStatus))
                setter.UpdateField(e => e.ShippingStatus, MapFromStateDto<VehicleState>(dto.ShippingStatus), new ShippingStatusHandler(dataService, _historyService));
            if (!string.IsNullOrEmpty(dto.DeliveryStatus))
                setter.UpdateField(e => e.DeliveryStatus, MapFromStateDto<VehicleState>(dto.DeliveryStatus), new DeliveryStatusHandler(dataService, _historyService));
            setter.UpdateField(e => e.OrderNumber, dto.OrderNumber, new OrderNumberHandler(_historyService));
            setter.UpdateField(e => e.OrderDate, ParseDateTime(dto.OrderDate));
            setter.UpdateField(e => e.OrderType, string.IsNullOrEmpty(dto.OrderType) ? (OrderType?)null : MapFromStateDto<OrderType>(dto.OrderType));
            setter.UpdateField(e => e.Payer, dto.Payer);
            setter.UpdateField(e => e.ClientName, dto.ClientName);
            setter.UpdateField(e => e.SoldTo, dto.SoldTo, new SoldToHandler(dataService, _historyService));
            setter.UpdateField(e => e.TemperatureMin, dto.TemperatureMin);
            setter.UpdateField(e => e.TemperatureMax, dto.TemperatureMax);
            setter.UpdateField(e => e.ShippingDate, ParseDateTime(dto.ShippingDate), new ShippingDateHandler(dataService, _historyService));
            setter.UpdateField(e => e.TransitDays, dto.TransitDays);
            setter.UpdateField(e => e.DeliveryDate, ParseDateTime(dto.DeliveryDate), new DeliveryDateHandler(dataService, _historyService));
            setter.UpdateField(e => e.BDFInvoiceNumber, dto.BDFInvoiceNumber);
            setter.UpdateField(e => e.ArticlesCount, dto.ArticlesCount);
            setter.UpdateField(e => e.BoxesCount, dto.BoxesCount);
            setter.UpdateField(e => e.ConfirmedBoxesCount, dto.ConfirmedBoxesCount);
            setter.UpdateField(e => e.PalletsCount, dto.PalletsCount, new PalletsCountHandler(dataService, _historyService));
            setter.UpdateField(e => e.ConfirmedPalletsCount, dto.ConfirmedPalletsCount, new ConfirmedPalletsCountHandler(dataService, _historyService));
            setter.UpdateField(e => e.ActualPalletsCount, dto.ActualPalletsCount, new ActualPalletsCountHandler(dataService, _historyService));
            setter.UpdateField(e => e.WeightKg, dto.WeightKg, new WeightKgHandler(dataService, _historyService));
            setter.UpdateField(e => e.ActualWeightKg, dto.ActualWeightKg, new ActualWeightKgHandler(dataService, _historyService));
            setter.UpdateField(e => e.OrderAmountExcludingVAT, dto.OrderAmountExcludingVAT);
            setter.UpdateField(e => e.InvoiceAmountExcludingVAT, dto.InvoiceAmountExcludingVAT);
            setter.UpdateField(e => e.DeliveryRegion, dto.DeliveryRegion);
            setter.UpdateField(e => e.DeliveryCity, dto.DeliveryCity);
            setter.UpdateField(e => e.ShippingAddress, dto.ShippingAddress);
            setter.UpdateField(e => e.DeliveryAddress, dto.DeliveryAddress);
            setter.UpdateField(e => e.ClientAvisationTime, ParseTime(dto.ClientAvisationTime));
            setter.UpdateField(e => e.OrderComments, dto.OrderComments);
            setter.UpdateField(e => e.PickingTypeId, string.IsNullOrEmpty(dto.PickingTypeId) ? (Guid?)null : Guid.Parse(dto.PickingTypeId), nameLoader: GetPickingTypeNameById);
            setter.UpdateField(e => e.PlannedArrivalTimeSlotBDFWarehouse, dto.PlannedArrivalTimeSlotBDFWarehouse);
            setter.UpdateField(e => e.LoadingArrivalTime, ParseDateTime(dto.LoadingArrivalTime), new LoadingArrivalTimeHandler(dataService, _historyService));
            setter.UpdateField(e => e.LoadingDepartureTime, ParseDateTime(dto.LoadingDepartureTime), new LoadingDepartureTimeHandler(dataService, _historyService));
            setter.UpdateField(e => e.UnloadingArrivalTime, ParseDateTime(dto.UnloadingArrivalDate)?.Add(ParseTime(dto.UnloadingArrivalTime) ?? TimeSpan.Zero), new UnloadingArrivalTimeHandler(dataService, _historyService));
            setter.UpdateField(e => e.UnloadingDepartureTime, ParseDateTime(dto.UnloadingDepartureDate)?.Add(ParseTime(dto.UnloadingDepartureTime) ?? TimeSpan.Zero), new UnloadingDepartureTimeHandler(dataService, _historyService));
            setter.UpdateField(e => e.DocumentsReturnDate, ParseDateTime(dto.DocumentsReturnDate));
            setter.UpdateField(e => e.ActualDocumentsReturnDate, ParseDateTime(dto.ActualDocumentsReturnDate));
            setter.UpdateField(e => e.WaybillTorg12, dto.WaybillTorg12 ?? false);
            setter.UpdateField(e => e.Invoice, dto.Invoice ?? false);
            setter.UpdateField(e => e.TrucksDowntime, dto.TrucksDowntime, new TrucksDowntimeHandler(dataService, _historyService));
            setter.UpdateField(e => e.ReturnInformation, dto.ReturnInformation);
            setter.UpdateField(e => e.ReturnShippingAccountNo, dto.ReturnShippingAccountNo);
            setter.UpdateField(e => e.PlannedReturnDate, ParseDateTime(dto.PlannedReturnDate));
            setter.UpdateField(e => e.ActualReturnDate, ParseDateTime(dto.ActualReturnDate));
            setter.UpdateField(e => e.MajorAdoptionNumber, dto.MajorAdoptionNumber);
            setter.UpdateField(e => e.OrderCreationDate, ParseDateTime(dto.OrderCreationDate));
            /*end of map dto to entity fields*/

            bool isNew = string.IsNullOrEmpty(dto.Id);
            if (isNew)
            {
                InitializeNewOrder(setter);
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

            setter.SaveHistoryLog();

            string errors = setter.ValidationErrors;
            return new ValidateResult(errors, entity.Id.ToString());
        }

        public override ValidateResult MapFromFormDtoToEntity(Order entity, OrderFormDto dto)
        {
            ValidateResult result = MapFromDtoToEntity(entity, dto);
            if (!result.IsError)
            {
                SaveItems(entity, dto);
            }
            return result;
        }

        public override OrderDto MapFromEntityToDto(Order entity)
        {
            return _mapper.Map<OrderDto>(entity);
        }

        public override OrderFormDto MapFromEntityToFormDto(Order entity)
        {
            OrderDto dto = _mapper.Map<OrderDto>(entity);
            OrderFormDto result = _mapper.Map<OrderFormDto>(dto);

            if (entity.ShippingId != null)
            {
                var shipping = dataService.GetById<Shipping>(entity.ShippingId.Value);
                result.ShippingNumber = shipping?.ShippingNumber;
            }

            result.Items = dataService.GetDbSet<OrderItem>()
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
            return id == null ? null : dataService.GetById<PickingType>(id.Value)?.Name;
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

        private void InitializeNewOrder(FieldSetter<Order> setter)
        {
            if (string.IsNullOrEmpty(setter.Entity.ShippingAddress))
            {
                var fromWarehouse = dataService.GetDbSet<Warehouse>().FirstOrDefault(x => !x.CustomerWarehouse);
                if (fromWarehouse != null)
                {
                    setter.UpdateField(e => e.ShippingWarehouseId, fromWarehouse.Id, ignoreChanges: true);
                    setter.UpdateField(e => e.ShippingAddress, fromWarehouse.Address);
                }
            }

            setter.UpdateField(e => e.Status, OrderState.Draft, ignoreChanges: true);
            setter.UpdateField(e => e.OrderCreationDate, DateTime.Today, ignoreChanges: true);
            setter.UpdateField(e => e.ShippingStatus, VehicleState.VehicleEmpty);
            setter.UpdateField(e => e.DeliveryStatus, VehicleState.VehicleEmpty);
        }

        private void SaveItems(Order entity, OrderFormDto dto)
        {
            if (dto.Items != null)
            {
                HashSet<Guid> updatedItems = new HashSet<Guid>();
                List<OrderItem> entityItems = dataService.GetDbSet<OrderItem>().Where(i => i.OrderId == entity.Id).ToList();
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
                        MapFromItemDtoToEntity(item, itemDto, true);
                        dataService.GetDbSet<OrderItem>().Add(item);

                        _historyService.Save(entity.Id, "orderItemAdded", item.Nart, item.Quantity);
                    }
                    else
                    {
                        updatedItems.Add(item.Id);
                        MapFromItemDtoToEntity(item, itemDto, false);
                        dataService.GetDbSet<OrderItem>().Update(item);
                    }
                }

                var itemsToRemove = entityItems.Where(i => !updatedItems.Contains(i.Id));
                foreach (var item in itemsToRemove)
                {
                    _historyService.Save(entity.Id, "orderItemRemoved", item.Nart, item.Quantity);
                }
                dataService.GetDbSet<OrderItem>().RemoveRange(itemsToRemove);

                entity.ArticlesCount = dto.Items.Count;
            }
        }

        private void MapFromItemDtoToEntity(OrderItem entity, OrderItemDto dto, bool isNew)
        {
            var setter = new FieldSetter<OrderItem>(entity, _historyService);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id), ignoreChanges: true);
            setter.UpdateField(x => x.Nart, dto.Nart, new OrderItemNartHandler(dataService, _historyService, isNew));
            setter.UpdateField(x => x.Quantity, dto.Quantity ?? 0, new OrderItemQuantityHandler(_historyService, isNew));

            setter.ApplyAfterActions();
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrderDto, OrderFormDto>();

                cfg.CreateMap<Order, OrderDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.Status, e => e.MapFrom((s, t) => s.Status.ToString().ToLowerfirstLetter()))
                    .ForMember(t => t.OrderType, e => e.MapFrom((s, t) => s.OrderType?.ToString()?.ToLowerfirstLetter()))
                    .ForMember(t => t.OrderDate, e => e.MapFrom((s, t) => s.OrderDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ShippingStatus, e => e.MapFrom((s, t) => s.ShippingStatus.ToString().ToLowerfirstLetter()))
                    .ForMember(t => t.DeliveryStatus, e => e.MapFrom((s, t) => s.DeliveryStatus.ToString().ToLowerfirstLetter()))
                    .ForMember(t => t.OrderShippingStatus, e => e.MapFrom((s, t) => s.OrderShippingStatus?.ToString()?.ToLowerfirstLetter()))
                    .ForMember(t => t.PickingTypeId, e => e.MapFrom((s, t) => s.PickingTypeId?.ToString()))
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
                    .ForMember(t => t.OrderCreationDate, e => e.MapFrom((s, t) => s.OrderCreationDate?.ToString("dd.MM.yyyy HH:mm")));

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
            // OrderNumber Filter
            query = query.ApplyStringFilter(i => i.OrderNumber, searchForm.Filter.OrderNumber);

            // OrderDate Filter
            query = query.ApplyDateRangeFilter(i => i.OrderDate.Value, searchForm.Filter.OrderDate);

            // OrderType Filter
            query = query.ApplyEnumFilter(i => i.OrderType.Value, searchForm.Filter.OrderType);

            // SoldTo Filter
            query = query.ApplyOptionsFilter(i => i.SoldTo, searchForm.Filter.SoldTo);

            // PickingTypeId Filter
            query = query.ApplyOptionsFilter(i => i.PickingTypeId, searchForm.Filter.PickingTypeId, i => new Guid(i));

            // Payer Filter
            query = query.ApplyStringFilter(i => i.Payer, searchForm.Filter.Payer);

            // Temperature Filters
            query = query
            .ApplyNumericFilter(i => i.TemperatureMin.Value, searchForm.Filter.TemperatureMin)
            .ApplyNumericFilter(i => i.TemperatureMax.Value, searchForm.Filter.TemperatureMax);

            // TransitDays Filter
            query = query.ApplyNumericFilter(i => i.TransitDays.Value, searchForm.Filter.TransitDays);

            // Shipping Filters
            query = query
                .ApplyStringFilter(i => i.ShippingAddress, searchForm.Filter.ShippingAddress)
                .ApplyDateRangeFilter(i => i.ShippingDate.Value, searchForm.Filter.ShippingDate);

            // Delivery Filters
            query = query
                .ApplyStringFilter(i => i.DeliveryCity, searchForm.Filter.DeliveryCity)
                .ApplyStringFilter(i => i.DeliveryRegion, searchForm.Filter.DeliveryRegion)
                .ApplyStringFilter(i => i.DeliveryAddress, searchForm.Filter.DeliveryAddress)
                .ApplyDateRangeFilter(i => i.DeliveryDate.Value, searchForm.Filter.DeliveryDate);

            query = query
                .ApplyNumericFilter(i => i.ArticlesCount.Value, searchForm.Filter.ArticlesCount)
                .ApplyNumericFilter(i => i.BoxesCount.Value, searchForm.Filter.BoxesCount)
                .ApplyNumericFilter(i => i.ConfirmedBoxesCount.Value, searchForm.Filter.ConfirmedBoxesCount)
                .ApplyNumericFilter(i => i.PalletsCount.Value, searchForm.Filter.PalletsCount)
                .ApplyNumericFilter(i => i.ActualPalletsCount.Value, searchForm.Filter.ActualPalletsCount)
                .ApplyNumericFilter(i => i.WeightKg.Value, searchForm.Filter.WeightKg)
                .ApplyNumericFilter(i => i.ActualWeightKg.Value, searchForm.Filter.ActualWeightKg)
                .ApplyNumericFilter(i => i.OrderAmountExcludingVAT.Value, searchForm.Filter.OrderAmountExcludingVAT)
                .ApplyStringFilter(i => i.BDFInvoiceNumber, searchForm.Filter.BdfInvoiceNumber)
                .ApplyDateRangeFilter(i => i.LoadingArrivalTime.Value, searchForm.Filter.LoadingArrivalTime)
                .ApplyDateRangeFilter(i => i.LoadingDepartureTime.Value, searchForm.Filter.LoadingDepartureTime)

                .ApplyDateRangeFilter(i => i.UnloadingArrivalTime.Value.Date, searchForm.Filter.UnloadingArrivalDate)
                // TODO: add filters for UnloadingArrivalTime
                //.ApplyDateRangeFilter(i => i.UnloadingArrivalTime.Value, searchForm.Filter.UnloadingArrivalTime);

                .ApplyDateRangeFilter(i => i.UnloadingDepartureTime.Value.Date, searchForm.Filter.UnloadingDepartureDate)
                // TODO: add filters for UnloadingDepartureTime

                .ApplyNumericFilter(i => i.TrucksDowntime.Value, searchForm.Filter.TrucksDowntime)
                .ApplyStringFilter(i => i.ReturnInformation, searchForm.Filter.ReturnInformation)
                .ApplyStringFilter(i => i.ReturnShippingAccountNo, searchForm.Filter.ReturnShippingAccountNo)
                .ApplyDateRangeFilter(i => i.PlannedReturnDate.Value, searchForm.Filter.PlannedReturnDate)
                .ApplyStringFilter(i => i.MajorAdoptionNumber, searchForm.Filter.MajorAdoptionNumber)
                //TODO: Apply ClientAvisationTime time filter
                //.ApplyStringFilter(i => i.ClientAvisationTime)

                .ApplyStringFilter(i => i.OrderComments, searchForm.Filter.OrderComments)
                .ApplyDateRangeFilter(i => i.OrderCreationDate.Value, searchForm.Filter.OrderCreationDate)
                .ApplyOptionsFilter(i => i.ShippingId.Value.ToString(), searchForm.Filter.ShippingId);

            return query;
        }
    }
}
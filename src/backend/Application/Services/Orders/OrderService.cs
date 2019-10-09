using Application.BusinessModels.Orders.Actions;
using Application.BusinessModels.Orders.Handlers;
using Application.Shared;
using AutoMapper;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Orders;
using Domain.Services.UserIdProvider;
using Domain.Shared;
using Domain.Shared.FormFilters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Orders
{
    public class OrdersService : GridWithDocumentsBase<Order, OrderDto, OrderFormDto, FilterFormDto<OrderFilterDto>>, IOrdersService
    {
        public OrdersService(AppDbContext appDbContext, IUserIdProvider userIdProvider) : base(appDbContext, userIdProvider)
        {
            _mapper = ConfigureMapper().CreateMapper();
        }

        public override DbSet<Order> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Orders;
        }

        public override IEnumerable<IAction<Order>> Actions()
        {
            return new List<IAction<Order>>
            {
                new CreateShipping(db),
                new CancelOrder(db),
                new RemoveFromShipping(db),
                new SendToArchive(db),
                new RecordFactOfLoss(db),
                new OrderShipped(db),
                new OrderDelivered(db),
                new FullReject(db),
                /*end of add single actions*/
            };
        }

        public override IEnumerable<IAction<IEnumerable<Order>>> GroupActions()
        {
            return new List<IAction<IEnumerable<Order>>>
            {
                new UnionOrders(db),
                new CancelOrders(db),
                new CreateShippingForeach(db),
                /*end of add group actions*/
            };
        }

        public override ValidateResult MapFromDtoToEntity(Order entity, OrderDto dto)
        {
            var setter = new FieldSetter<Order>(entity);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id));
            if (!string.IsNullOrEmpty(dto.Status))
                setter.UpdateField(e => e.Status, MapFromStateDto<OrderState>(dto.Status));
            if (!string.IsNullOrEmpty(dto.ShippingStatus))
                setter.UpdateField(e => e.ShippingStatus, MapFromStateDto<VehicleState>(dto.ShippingStatus), new ShippingStatusHandler(db));
            if (!string.IsNullOrEmpty(dto.DeliveryStatus))
                setter.UpdateField(e => e.DeliveryStatus, MapFromStateDto<VehicleState>(dto.DeliveryStatus), new DeliveryStatusHandler(db));
            setter.UpdateField(e => e.OrderNumber, dto.OrderNumber, new OrderNumberHandler());
            setter.UpdateField(e => e.OrderDate, ParseDateTime(dto.OrderDate));
            setter.UpdateField(e => e.OrderType, string.IsNullOrEmpty(dto.OrderType) ? (OrderType?)null : MapFromStateDto<OrderType>(dto.OrderType));
            setter.UpdateField(e => e.Payer, dto.Payer);
            setter.UpdateField(e => e.ClientName, dto.ClientName);
            setter.UpdateField(e => e.SoldTo, dto.SoldTo, new SoldToHandler(db));
            setter.UpdateField(e => e.TemperatureMin, dto.TemperatureMin);
            setter.UpdateField(e => e.TemperatureMax, dto.TemperatureMax);
            setter.UpdateField(e => e.ShippingDate, ParseDateTime(dto.ShippingDate), new ShippingDateHandler(db));
            setter.UpdateField(e => e.TransitDays, dto.TransitDays);
            setter.UpdateField(e => e.DeliveryDate, ParseDateTime(dto.DeliveryDate), new DeliveryDateHandler(db));
            setter.UpdateField(e => e.BDFInvoiceNumber, dto.BDFInvoiceNumber);
            setter.UpdateField(e => e.ArticlesCount, dto.ArticlesCount);
            setter.UpdateField(e => e.BoxesCount, dto.BoxesCount);
            setter.UpdateField(e => e.ConfirmedBoxesCount, dto.ConfirmedBoxesCount);
            setter.UpdateField(e => e.PalletsCount, dto.PalletsCount, new PalletsCountHandler(db));
            setter.UpdateField(e => e.ConfirmedPalletsCount, dto.ConfirmedPalletsCount, new ConfirmedPalletsCountHandler(db));
            setter.UpdateField(e => e.ActualPalletsCount, dto.ActualPalletsCount, new ActualPalletsCountHandler(db));
            setter.UpdateField(e => e.WeightKg, dto.WeightKg, new WeightKgHandler(db));
            setter.UpdateField(e => e.ActualWeightKg, dto.ActualWeightKg, new ActualWeightKgHandler(db));
            setter.UpdateField(e => e.OrderAmountExcludingVAT, dto.OrderAmountExcludingVAT);
            setter.UpdateField(e => e.InvoiceAmountExcludingVAT, dto.InvoiceAmountExcludingVAT);
            setter.UpdateField(e => e.DeliveryRegion, dto.DeliveryRegion);
            setter.UpdateField(e => e.DeliveryCity, dto.DeliveryCity);
            setter.UpdateField(e => e.ShippingAddress, dto.ShippingAddress);
            setter.UpdateField(e => e.DeliveryAddress, dto.DeliveryAddress);
            setter.UpdateField(e => e.ClientAvisationTime, ParseTime(dto.ClientAvisationTime));
            setter.UpdateField(e => e.OrderComments, dto.OrderComments);
            setter.UpdateField(e => e.PickingTypeId, string.IsNullOrEmpty(dto.PickingTypeId) ? (Guid?)null : Guid.Parse(dto.PickingTypeId));
            setter.UpdateField(e => e.PlannedArrivalTimeSlotBDFWarehouse, dto.PlannedArrivalTimeSlotBDFWarehouse);
            setter.UpdateField(e => e.LoadingArrivalTime, ParseDateTime(dto.LoadingArrivalTime), new LoadingArrivalTimeHandler(db));
            setter.UpdateField(e => e.LoadingDepartureTime, ParseDateTime(dto.LoadingDepartureTime), new LoadingDepartureTimeHandler(db));
            setter.UpdateField(e => e.UnloadingArrivalTime, ParseDateTime(dto.UnloadingArrivalDate)?.Add(ParseTime(dto.UnloadingArrivalTime) ?? TimeSpan.Zero), new UnloadingArrivalTimeHandler(db));
            setter.UpdateField(e => e.UnloadingDepartureTime, ParseDateTime(dto.UnloadingDepartureDate)?.Add(ParseTime(dto.UnloadingDepartureTime) ?? TimeSpan.Zero), new UnloadingDepartureTimeHandler(db));
            setter.UpdateField(e => e.TrucksDowntime, dto.TrucksDowntime, new TrucksDowntimeHandler(db));
            setter.UpdateField(e => e.ReturnInformation, dto.ReturnInformation);
            setter.UpdateField(e => e.ReturnShippingAccountNo, dto.ReturnShippingAccountNo);
            setter.UpdateField(e => e.PlannedReturnDate, ParseDateTime(dto.PlannedReturnDate));
            setter.UpdateField(e => e.ActualReturnDate, ParseDateTime(dto.ActualReturnDate));
            setter.UpdateField(e => e.MajorAdoptionNumber, dto.MajorAdoptionNumber);
            setter.UpdateField(e => e.OrderCreationDate, ParseDateTime(dto.OrderCreationDate));
            if (!string.IsNullOrEmpty(dto.ShippingId))
                setter.UpdateField(e => e.ShippingId, Guid.Parse(dto.ShippingId));
            /*end of map dto to entity fields*/

            if (string.IsNullOrEmpty(dto.Id))
            {
                InitializeNewOrder(entity);
            }

            setter.ApplyAfterActions();

            if (setter.HasChanges)
            {
                CheckRequiredFields(entity);
            }

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
            result.Items = db.OrderItems.Where(i => i.OrderId == entity.Id).Select(MapFromItemEntityToDto).ToList();
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

        private void CheckRequiredFields(Order order)
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
                }
            }
        }

        private void InitializeNewOrder(Order order)
        {
            if (string.IsNullOrEmpty(order.ShippingAddress))
            {
                var fromWarehouse = db.Warehouses.FirstOrDefault(x => x.CustomerWarehouse == "Нет");
                if (fromWarehouse != null)
                {
                    order.ShippingWarehouseId = fromWarehouse.Id;
                    order.ShippingAddress = fromWarehouse.Address;
                }
            }

            order.Status = OrderState.Draft;
            order.OrderCreationDate = DateTime.Today;
            order.ShippingStatus = VehicleState.VehicleEmpty;
            order.DeliveryStatus = VehicleState.VehicleEmpty;
        }

        private void SaveItems(Order entity, OrderFormDto dto)
        {
            if (dto.Items != null)
            {
                HashSet<Guid> updatedItems = new HashSet<Guid>();
                List<OrderItem> entityItems = db.OrderItems.Where(i => i.OrderId == entity.Id).ToList();
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
                        MapFromItemDtoToEntity(item, itemDto);
                        db.OrderItems.Add(item);
                    }
                    else
                    {
                        updatedItems.Add(item.Id);
                        MapFromItemDtoToEntity(item, itemDto);
                        db.OrderItems.Update(item);
                    }
                }

                var itemsToRemove = entityItems.Where(i => !updatedItems.Contains(i.Id));
                db.OrderItems.RemoveRange(itemsToRemove);

                entity.ArticlesCount = dto.Items.Count;
            }
        }

        private void MapFromItemDtoToEntity(OrderItem entity, OrderItemDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Nart = dto.Nart;
            entity.Quantity = dto.Quantity ?? 0;
        }

        private OrderItemDto MapFromItemEntityToDto(OrderItem entity)
        {
            return new OrderItemDto
            {
                Id = entity.Id.ToString(),
                Nart = entity.Nart,
                Quantity = entity.Quantity
            };
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrderDto, OrderFormDto>();

                cfg.CreateMap<Order, OrderDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.Status, e => e.MapFrom((s, t) => s.Status.ToString().ToLowerfirstLetter()))
                    .ForMember(t => t.OrderType, e => e.MapFrom((s, t) => s.OrderType.ToString()))
                    .ForMember(t => t.OrderDate, e => e.MapFrom((s, t) => s.OrderDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ShippingStatus, e => e.MapFrom((s, t) => s.ShippingStatus.ToString().ToLowerfirstLetter()))
                    .ForMember(t => t.DeliveryStatus, e => e.MapFrom((s, t) => s.DeliveryStatus.ToString().ToLowerfirstLetter()))
                    .ForMember(t => t.PickingTypeId, e => e.MapFrom((s, t) => s.PickingTypeId?.ToString()))
                    .ForMember(t => t.ShippingDate, e => e.MapFrom((s, t) => s.ShippingDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.DeliveryDate, e => e.MapFrom((s, t) => s.DeliveryDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.LoadingArrivalTime, e => e.MapFrom((s, t) => s.LoadingArrivalTime?.ToString("dd.MM.yyyy HH:mm")))
                    .ForMember(t => t.LoadingDepartureTime, e => e.MapFrom((s, t) => s.LoadingDepartureTime?.ToString("dd.MM.yyyy HH:mm")))
                    .ForMember(t => t.UnloadingArrivalDate, e => e.MapFrom((s, t) => s.UnloadingArrivalTime?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.UnloadingArrivalTime, e => e.MapFrom((s, t) => s.UnloadingArrivalTime?.ToString("HH:mm")))
                    .ForMember(t => t.UnloadingDepartureDate, e => e.MapFrom((s, t) => s.UnloadingDepartureTime?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.UnloadingDepartureTime, e => e.MapFrom((s, t) => s.UnloadingDepartureTime?.ToString("HH:mm")))
                    .ForMember(t => t.PlannedReturnDate, e => e.MapFrom((s, t) => s.PlannedReturnDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.ActualReturnDate, e => e.MapFrom((s, t) => s.ActualReturnDate?.ToString("dd.MM.yyyy")))
                    .ForMember(t => t.OrderCreationDate, e => e.MapFrom((s, t) => s.OrderCreationDate?.ToString("dd.MM.yyyy HH:mm")));
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

            if (!string.IsNullOrEmpty(searchForm.Filter.OrderNumber))
            {
                query = query.Where(i => i.OrderNumber == searchForm.Filter.OrderNumber);
            }

            // OrderDate Filter

            if (!string.IsNullOrEmpty(searchForm.Filter.OrderDate))
            {
                var dates = searchForm.Filter.OrderDate.Split("-");

                var fromDateStr = dates.FirstOrDefault();
                var toDateStr = dates.ElementAtOrDefault(1);

                if (DateTime.TryParse(fromDateStr, out DateTime fromDate))
                {
                    query = query.Where(i => i.OrderDate >= fromDate);
                }

                if (DateTime.TryParse(toDateStr, out DateTime toDate))
                {
                    query = query.Where(i => i.OrderDate <= toDate);
                }
            }

            // OrderType Filter

            if (!string.IsNullOrEmpty(searchForm.Filter.OrderType))
            {
                var types = searchForm.Filter.OrderType.Split("|")
                    .Select(i => MapFromStateDto<OrderType>(i));

                query = query.Where(i => i.OrderType.HasValue && types.Contains(i.OrderType.Value));
            }

            // SoldTo Filter

            if (!string.IsNullOrEmpty(searchForm.Filter.SoldTo))
            {
                var solds = searchForm.Filter.SoldTo.Split("|");
                query = query.Where(i => solds.Contains(i.SoldTo));
            }

            // PickingTypeId Filter
            if (!string.IsNullOrEmpty(searchForm.Filter.PickingTypeId))
            {
                var pickingTypes = searchForm.Filter.PickingTypeId.Split("|").Select(i => new Guid(i));
                query = query.Where(i => i.PickingTypeId.HasValue && pickingTypes.Contains(i.PickingTypeId.Value));
            }

            // Payer Filter

            if (!string.IsNullOrEmpty(searchForm.Filter.Payer))
            {
                query = query.Where(i => i.Payer.Contains(searchForm.Filter.Payer));
            }

            // TemperatureMin Filter

            if (int.TryParse(searchForm.Filter.TemperatureMin, out int temperatureMin))
            {
                query = query.Where(i => i.TemperatureMin == temperatureMin);
            }

            // TemperatureMax Filter

            if (int.TryParse(searchForm.Filter.TemperatureMax, out int temperatureMax))
            {
                query = query.Where(i => i.TemperatureMax == temperatureMax);
            }

            // TransitDays Filter

            if (int.TryParse(searchForm.Filter.TransitDays, out int transitDays))
            {
                query = query.Where(i => i.TransitDays == transitDays);
            }


            // ShippingAddress Filter

            if (!string.IsNullOrEmpty(searchForm.Filter.ShippingAddress))
            {
                query = query.Where(i => i.ShippingAddress.Contains(searchForm.Filter.ShippingAddress));
            }

            // ShippingDate Filter

            if (!string.IsNullOrEmpty(searchForm.Filter.ShippingDate))
            {
                var dates = searchForm.Filter.ShippingDate.Split("-");

                var fromDateStr = dates.FirstOrDefault();
                var toDateStr = dates.ElementAtOrDefault(1);

                if (DateTime.TryParse(fromDateStr, out DateTime fromDate))
                {
                    query = query.Where(i => i.ShippingDate >= fromDate);
                }

                if (DateTime.TryParse(toDateStr, out DateTime toDate))
                {
                    query = query.Where(i => i.ShippingDate <= toDate);
                }
            }

            // DeliveryCity Filter

            if (!string.IsNullOrEmpty(searchForm.Filter.DeliveryCity))
            {
                query = query.Where(i => i.DeliveryCity.Contains(searchForm.Filter.DeliveryCity));
            }

            // DeliveryRegion Filter

            if (!string.IsNullOrEmpty(searchForm.Filter.DeliveryRegion))
            {
                query = query.Where(i => i.DeliveryRegion.Contains(searchForm.Filter.DeliveryRegion));
            }

            // DeliveryAddress Filter

            if (!string.IsNullOrEmpty(searchForm.Filter.DeliveryAddress))
            {
                query = query.Where(i => i.DeliveryAddress.Contains(searchForm.Filter.DeliveryAddress));
            }

            // DeliveryDate Filter

            if (!string.IsNullOrEmpty(searchForm.Filter.DeliveryDate))
            {
                var dates = searchForm.Filter.DeliveryDate.Split("-");

                var fromDateStr = dates.FirstOrDefault();
                var toDateStr = dates.ElementAtOrDefault(1);

                if (DateTime.TryParse(fromDateStr, out DateTime fromDate))
                {
                    query = query.Where(i => i.DeliveryDate >= fromDate);
                }

                if (DateTime.TryParse(toDateStr, out DateTime toDate))
                {
                    query = query.Where(i => i.DeliveryDate < toDate.AddDays(1));
                }
            }

            return query;
        }
    }
}
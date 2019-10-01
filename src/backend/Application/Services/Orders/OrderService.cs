using Application.Actions.Orders;
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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Orders
{
    public class OrdersService : GridWithDocumentsBase<Order, OrderDto, OrderFormDto>, IOrdersService
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

        public override void MapFromDtoToEntity(Order entity, OrderDto dto)
        {
            var setter = new FieldSetter<Order>(entity);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id));
            setter.UpdateField(e => e.Status, string.IsNullOrEmpty(dto.Status) ? (OrderState?)null : MapFromStateDto<OrderState>(dto.Status));
            setter.UpdateField(e => e.OrderNumber, dto.OrderNumber, AfterOrderNumberChanged);
            setter.UpdateField(e => e.OrderDate, dto.OrderDate);
            setter.UpdateField(e => e.OrderType, string.IsNullOrEmpty(dto.OrderType) ? (OrderType?)null : MapFromStateDto<OrderType>(dto.OrderType));
            setter.UpdateField(e => e.Payer, dto.Payer);
            setter.UpdateField(e => e.ClientName, dto.ClientName);
            setter.UpdateField(e => e.SoldTo, dto.SoldTo, AfterSoldToChanged);
            setter.UpdateField(e => e.TemperatureMin, dto.TemperatureMin);
            setter.UpdateField(e => e.TemperatureMax, dto.TemperatureMax);
            setter.UpdateField(e => e.ShippingDate, dto.ShippingDate);
            setter.UpdateField(e => e.TransitDays, dto.TransitDays);
            setter.UpdateField(e => e.DeliveryDate, dto.DeliveryDate);
            setter.UpdateField(e => e.BDFInvoiceNumber, dto.BDFInvoiceNumber);
            setter.UpdateField(e => e.InvoiceNumber, dto.InvoiceNumber);
            setter.UpdateField(e => e.ArticlesCount, dto.ArticlesCount);
            setter.UpdateField(e => e.BoxesCount, dto.BoxesCount);
            setter.UpdateField(e => e.ConfirmedBoxesCount, dto.ConfirmedBoxesCount);
            setter.UpdateField(e => e.PalletsCount, dto.PalletsCount);
            setter.UpdateField(e => e.ConfirmedPalletsCount, dto.ConfirmedPalletsCount);
            setter.UpdateField(e => e.ActualPalletsCount, dto.ActualPalletsCount);
            setter.UpdateField(e => e.WeightKg, dto.WeightKg);
            setter.UpdateField(e => e.ActualWeightKg, dto.ActualWeightKg);
            setter.UpdateField(e => e.OrderAmountExcludingVAT, dto.OrderAmountExcludingVAT);
            setter.UpdateField(e => e.InvoiceAmountExcludingVAT, dto.InvoiceAmountExcludingVAT);
            setter.UpdateField(e => e.Region, dto.Region);
            setter.UpdateField(e => e.City, dto.City);
            setter.UpdateField(e => e.ShippingAddress, dto.ShippingAddress);
            setter.UpdateField(e => e.DeliveryAddress, dto.DeliveryAddress);
            setter.UpdateField(e => e.ClientAvisationTime, dto.ClientAvisationTime);
            setter.UpdateField(e => e.OrderComments, dto.OrderComments);
            setter.UpdateField(e => e.PickingType, dto.PickingType);
            setter.UpdateField(e => e.PlannedArrivalTimeSlotBDFWarehouse, dto.PlannedArrivalTimeSlotBDFWarehouse);
            setter.UpdateField(e => e.ArrivalTimeForLoadingBDFWarehouse, dto.ArrivalTimeForLoadingBDFWarehouse);
            setter.UpdateField(e => e.DepartureTimeFromTheBDFWarehouse, dto.DepartureTimeFromTheBDFWarehouse);
            setter.UpdateField(e => e.ActualDateOfArrivalAtTheConsignee, dto.ActualDateOfArrivalAtTheConsignee);
            setter.UpdateField(e => e.ArrivalTimeToConsignee, dto.ArrivalTimeToConsignee);
            setter.UpdateField(e => e.DateOfDepartureFromTheConsignee, dto.DateOfDepartureFromTheConsignee);
            setter.UpdateField(e => e.DepartureTimeFromConsignee, dto.DepartureTimeFromConsignee);
            setter.UpdateField(e => e.TrucksDowntime, dto.TrucksDowntime);
            setter.UpdateField(e => e.ReturnInformation, dto.ReturnInformation);
            setter.UpdateField(e => e.ReturnShippingAccountNo, dto.ReturnShippingAccountNo);
            setter.UpdateField(e => e.PlannedReturnDate, dto.PlannedReturnDate);
            setter.UpdateField(e => e.ActualReturnDate, dto.ActualReturnDate);
            setter.UpdateField(e => e.MajorAdoptionNumber, dto.MajorAdoptionNumber);
            setter.UpdateField(e => e.Avization, dto.Avization);
            setter.UpdateField(e => e.OrderItems, dto.OrderItems);
            setter.UpdateField(e => e.OrderCreationDate, dto.OrderCreationDate);
            if (!string.IsNullOrEmpty(dto.ShippingId))
                setter.UpdateField(e => e.ShippingId, Guid.Parse(dto.ShippingId));
            setter.UpdateField(e => e.Positions, dto.Positions);
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
        }

        public override void MapFromFormDtoToEntity(Order entity, OrderFormDto dto)
        {
            MapFromDtoToEntity(entity, dto);
            SaveItems(entity, dto);
        }

        public override OrderDto MapFromEntityToDto(Order entity)
        {
            return _mapper.Map<OrderDto>(entity);
        }

        public override OrderFormDto MapFromEntityToFormDto(Order entity)
        {
            OrderFormDto result = _mapper.Map<OrderFormDto>(entity);
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

        private void AfterOrderNumberChanged(Order order, string oldValue, string newValue)
        {
            if (order.OrderNumber.StartsWith("2"))
                order.OrderType = OrderType.FD;
            else 
                order.OrderType = OrderType.OR;
        }

        private void AfterSoldToChanged(Order order, string oldValue, string newValue)
        {
            if (!string.IsNullOrEmpty(order.SoldTo))
            {
                var soldToWarehouse = db.Warehouses.FirstOrDefault(x => x.SoldToNumber == order.SoldTo);
                if (soldToWarehouse != null)
                {
                    order.ClientName = soldToWarehouse.TheNameOfTheWarehouse;

                    if (soldToWarehouse.UsePickingType == "��")
                        order.PickingType = soldToWarehouse.PickingType;

                    if (!string.IsNullOrEmpty(soldToWarehouse.LeadtimeDays))
                    {
                        int leadTimeDays = int.Parse(soldToWarehouse.LeadtimeDays);
                        order.TransitDays = leadTimeDays;
                    }

                    order.ShippingDate = order.DeliveryDate?.AddDays(0 - order.TransitDays ?? 0);

                    order.DeliveryAddress = soldToWarehouse.Address;
                    order.City = soldToWarehouse.City;
                    order.Region = soldToWarehouse.Region;
                }
            }
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
                var fromWarehouse = db.Warehouses.FirstOrDefault(x => x.CustomerWarehouse == "���");
                if (fromWarehouse != null)
                {
                    order.ShippingAddress = fromWarehouse.Address;
                }
            }

            order.Status = OrderState.Draft;
            order.OrderCreationDate = DateTime.Today;
        }

        private void SaveItems(Order entity, OrderFormDto dto)
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

        private void MapFromItemDtoToEntity(OrderItem entity, OrderItemDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Nart = dto.Nart;
            entity.Quantity = dto.Quantity;
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
                    .ForMember(t => t.Status, e => e.MapFrom((s, t) => s.Status.ToString().ToLowerfirstLetter()));

                cfg.CreateMap<Order, OrderFormDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.Status, e => e.MapFrom((s, t) => s.Status.ToString().ToLowerfirstLetter()));
            });
            return result;
        }

        private readonly IMapper _mapper;
    }
}
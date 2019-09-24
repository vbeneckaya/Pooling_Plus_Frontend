using System;
using System.Collections.Generic;
using Application.Actions.Orders;
using Application.Shared;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Orders;
using Domain.Services.UserIdProvider;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Orders
{
    public class OrdersService : GridServiceBase<Order, OrderDto>, IOrdersService
    {
        public OrdersService(AppDbContext appDbContext, IUserIdProvider userIdProvider) : base(appDbContext, userIdProvider)
        {
        }

        public override DbSet<Order> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Orders;
        }

        public override IEnumerable<IAction<Order>> Actions()
        {
            return new List<IAction<Order>>
            {
                new Actions.Orders.Exception(db),
                new CreateShipping(db),
                new CancelOrder(db),
                new RemoveFromShipping(db),
                new SendToArchive(db),
                new RecordFactOfLoss(db),
                new SaveOrder(db),
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
                new SaveOrders(db),
                new CreateShippingForeach(db),
                /*end of add group actions*/
            };
        }

        public override void MapFromDtoToEntity(Order entity, OrderDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            if(!string.IsNullOrEmpty(dto.Status))
                entity.Status =  MapFromStateDto<OrderState>(dto.Status);
            entity.SalesOrderNumber = dto.SalesOrderNumber;
            entity.OrderDate = dto.OrderDate;
            entity.TypeOfOrder = dto.TypeOfOrder;
            entity.Payer = dto.Payer;
            entity.CustomerName = dto.CustomerName;
            entity.SoldTo = dto.SoldTo;
            entity.ShippingDate = dto.ShippingDate;
            entity.DaysOnTheRoad = dto.DaysOnTheRoad;
            entity.DeliveryDate = dto.DeliveryDate;
            entity.BDFInvoiceNumber = dto.BDFInvoiceNumber;
            entity.InvoiceNumber = dto.InvoiceNumber;
            entity.NumberOfArticles = dto.NumberOfArticles;
            entity.TheNumberOfBoxes = dto.TheNumberOfBoxes;
            entity.PreliminaryNumberOfPallets = dto.PreliminaryNumberOfPallets;
            entity.ActualNumberOfPallets = dto.ActualNumberOfPallets;
            entity.ConfirmedBoxes = dto.ConfirmedBoxes;
            entity.ConfirmedNumberOfPallets = dto.ConfirmedNumberOfPallets;
            entity.WeightKg = dto.WeightKg;
            entity.OrderAmountExcludingVAT = dto.OrderAmountExcludingVAT;
            entity.TTNAmountExcludingVAT = dto.TTNAmountExcludingVAT;
            entity.Region = dto.Region;
            entity.City = dto.City;
            entity.ShippingAddress = dto.ShippingAddress;
            entity.DeliveryAddress = dto.DeliveryAddress;
            entity.CustomerAvizTime = dto.CustomerAvizTime;
            entity.OrderComments = dto.OrderComments;
            entity.TypeOfEquipment = dto.TypeOfEquipment;
            entity.PlannedArrivalTimeSlotBDFWarehouse = dto.PlannedArrivalTimeSlotBDFWarehouse;
            entity.ArrivalTimeForLoadingBDFWarehouse = dto.ArrivalTimeForLoadingBDFWarehouse;
            entity.DepartureTimeFromTheBDFWarehouse = dto.DepartureTimeFromTheBDFWarehouse;
            entity.ActualDateOfArrivalAtTheConsignee = dto.ActualDateOfArrivalAtTheConsignee;
            entity.ArrivalTimeToConsignee = dto.ArrivalTimeToConsignee;
            entity.DateOfDepartureFromTheConsignee = dto.DateOfDepartureFromTheConsignee;
            entity.DepartureTimeFromConsignee = dto.DepartureTimeFromConsignee;
            entity.TheNumberOfHoursOfDowntime = dto.TheNumberOfHoursOfDowntime;
            entity.ReturnInformation = dto.ReturnInformation;
            entity.ReturnShippingAccountNo = dto.ReturnShippingAccountNo;
            entity.PlannedReturnDate = dto.PlannedReturnDate;
            entity.ActualReturnDate = dto.ActualReturnDate;
            entity.MajorAdoptionNumber = dto.MajorAdoptionNumber;
            entity.Avization = dto.Avization;
            entity.OrderItems = dto.OrderItems;
            entity.OrderCreationDate = dto.OrderCreationDate;
            if(!string.IsNullOrEmpty(dto.ShippingId))
                entity.ShippingId = Guid.Parse(dto.ShippingId);
            entity.Positions = dto.Positions;
            /*end of map dto to entity fields*/
        }

        public override OrderDto MapFromEntityToDto(Order entity)
        {
            return new OrderDto
            {
                Id = entity.Id.ToString(),
                Status = entity.Status.ToString().ToLowerfirstLetter(),
                SalesOrderNumber = entity.SalesOrderNumber,
                OrderDate = entity.OrderDate,
                TypeOfOrder = entity.TypeOfOrder,
                Payer = entity.Payer,
                CustomerName = entity.CustomerName,
                SoldTo = entity.SoldTo,
                ShippingDate = entity.ShippingDate,
                DaysOnTheRoad = entity.DaysOnTheRoad,
                DeliveryDate = entity.DeliveryDate,
                BDFInvoiceNumber = entity.BDFInvoiceNumber,
                InvoiceNumber = entity.InvoiceNumber,
                NumberOfArticles = entity.NumberOfArticles,
                TheNumberOfBoxes = entity.TheNumberOfBoxes,
                PreliminaryNumberOfPallets = entity.PreliminaryNumberOfPallets,
                ActualNumberOfPallets = entity.ActualNumberOfPallets,
                ConfirmedBoxes = entity.ConfirmedBoxes,
                ConfirmedNumberOfPallets = entity.ConfirmedNumberOfPallets,
                WeightKg = entity.WeightKg,
                OrderAmountExcludingVAT = entity.OrderAmountExcludingVAT,
                TTNAmountExcludingVAT = entity.TTNAmountExcludingVAT,
                Region = entity.Region,
                City = entity.City,
                ShippingAddress = entity.ShippingAddress,
                DeliveryAddress = entity.DeliveryAddress,
                CustomerAvizTime = entity.CustomerAvizTime,
                OrderComments = entity.OrderComments,
                TypeOfEquipment = entity.TypeOfEquipment,
                PlannedArrivalTimeSlotBDFWarehouse = entity.PlannedArrivalTimeSlotBDFWarehouse,
                ArrivalTimeForLoadingBDFWarehouse = entity.ArrivalTimeForLoadingBDFWarehouse,
                DepartureTimeFromTheBDFWarehouse = entity.DepartureTimeFromTheBDFWarehouse,
                ActualDateOfArrivalAtTheConsignee = entity.ActualDateOfArrivalAtTheConsignee,
                ArrivalTimeToConsignee = entity.ArrivalTimeToConsignee,
                DateOfDepartureFromTheConsignee = entity.DateOfDepartureFromTheConsignee,
                DepartureTimeFromConsignee = entity.DepartureTimeFromConsignee,
                TheNumberOfHoursOfDowntime = entity.TheNumberOfHoursOfDowntime,
                ReturnInformation = entity.ReturnInformation,
                ReturnShippingAccountNo = entity.ReturnShippingAccountNo,
                PlannedReturnDate = entity.PlannedReturnDate,
                ActualReturnDate = entity.ActualReturnDate,
                MajorAdoptionNumber = entity.MajorAdoptionNumber,
                Avization = entity.Avization,
                OrderItems = entity.OrderItems,
                OrderCreationDate = entity.OrderCreationDate,
                ShippingId = entity.ShippingId.ToString(),
                Positions = entity.Positions,
                /*end of map entity to dto fields*/
            };
        }
    }
}
using System;
using System.Collections.Generic;

namespace Domain.Services.Orders
{
    public class OrderDto : IDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string SalesOrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string TypeOfOrder { get; set; }
        public string Payer { get; set; }
        public string CustomerName { get; set; }
        public string SoldTo { get; set; }
        public DateTime? ShippingDate { get; set; }
        public int? DaysOnTheRoad { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string BDFInvoiceNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public int? NumberOfArticles { get; set; }
        public int? TheNumberOfBoxes { get; set; }
        public int? PreliminaryNumberOfPallets { get; set; }
        public int? ActualNumberOfPallets { get; set; }
        public int? ConfirmedBoxes { get; set; }
        public int? ConfirmedNumberOfPallets { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? OrderAmountExcludingVAT { get; set; }
        public decimal? TTNAmountExcludingVAT { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string ShippingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerAvizTime { get; set; }
        public string OrderComments { get; set; }
        public string TypeOfEquipment { get; set; }
        public string PlannedArrivalTimeSlotBDFWarehouse { get; set; }
        public string ArrivalTimeForLoadingBDFWarehouse { get; set; }
        public string DepartureTimeFromTheBDFWarehouse { get; set; }
        public string ActualDateOfArrivalAtTheConsignee { get; set; }
        public string ArrivalTimeToConsignee { get; set; }
        public string DateOfDepartureFromTheConsignee { get; set; }
        public string DepartureTimeFromConsignee { get; set; }
        public string TheNumberOfHoursOfDowntime { get; set; }
        public string ReturnInformation { get; set; }
        public string ReturnShippingAccountNo { get; set; }
        public string PlannedReturnDate { get; set; }
        public string ActualReturnDate { get; set; }
        public string MajorAdoptionNumber { get; set; }
        public string Avization { get; set; }
        public string OrderItems { get; set; }
        public string OrderCreationDate { get; set; }
        public string ShippingId { get; set; }

        public string Positions { get; set; }

        /*end of fields*/

        public List<OrderItemDto> Items { get; set; }
    }
}
using System;

namespace Domain.Services.Orders
{
    public class OrderDto : IDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderType { get; set; }
        public string Payer { get; set; }
        public string ClientName { get; set; }
        public string SoldTo { get; set; }
        public int? TemperatureMin { get; set; }
        public int? TemperatureMax { get; set; }
        public DateTime? ShippingDate { get; set; }
        public int? TransitDays { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string BDFInvoiceNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public int? ArticlesCount { get; set; }
        public int? BoxesCount { get; set; }
        public int? ConfirmedBoxesCount { get; set; }
        public int? PalletsCount { get; set; }
        public int? ConfirmedPalletsCount { get; set; }
        public int? ActualPalletsCount { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? ActualWeightKg { get; set; }
        public decimal? OrderAmountExcludingVAT { get; set; }
        public decimal? InvoiceAmountExcludingVAT { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string ShippingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public TimeSpan? ClientAvisationTime { get; set; }
        public string OrderComments { get; set; }
        public string PickingType { get; set; }
        public string PlannedArrivalTimeSlotBDFWarehouse { get; set; }
        public DateTime? ArrivalTimeForLoadingBDFWarehouse { get; set; }
        public DateTime? DepartureTimeFromTheBDFWarehouse { get; set; }
        public DateTime? ActualDateOfArrivalAtTheConsignee { get; set; }
        public TimeSpan? ArrivalTimeToConsignee { get; set; }
        public DateTime? DateOfDepartureFromTheConsignee { get; set; }
        public TimeSpan? DepartureTimeFromConsignee { get; set; }
        public decimal? TrucksDowntime { get; set; }
        public string ReturnInformation { get; set; }
        public string ReturnShippingAccountNo { get; set; }
        public DateTime? PlannedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public string MajorAdoptionNumber { get; set; }
        public string Avization { get; set; }
        public string OrderItems { get; set; }
        public DateTime? OrderCreationDate { get; set; }
        public string ShippingId { get; set; }

        public string Positions { get; set; }

        /*end of fields*/
    }
}
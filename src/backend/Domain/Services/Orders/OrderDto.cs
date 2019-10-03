using System;

namespace Domain.Services.Orders
{
    public class OrderDto : IDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string OrderType { get; set; }
        public string Payer { get; set; }
        public string ClientName { get; set; }
        public string SoldTo { get; set; }
        public int? TemperatureMin { get; set; }
        public int? TemperatureMax { get; set; }
        public string ShippingDate { get; set; }
        public int? TransitDays { get; set; }
        public string DeliveryDate { get; set; }
        public string BDFInvoiceNumber { get; set; }
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
        public string DeliveryRegion { get; set; }
        public string DeliveryCity { get; set; }
        public string ShippingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public string ShippingStatus { get; set; }
        public string DeliveryStatus { get; set; }
        public string ClientAvisationTime { get; set; }
        public string OrderComments { get; set; }
        public string PickingType { get; set; }
        public string PlannedArrivalTimeSlotBDFWarehouse { get; set; }
        public string LoadingArrivalTime { get; set; }
        public string LoadingDepartureTime { get; set; }
        public string UnloadingArrivalDate { get; set; }
        public string UnloadingArrivalTime { get; set; }
        public string UnloadingDepartureDate { get; set; }
        public string UnloadingDepartureTime { get; set; }
        public int? TrucksDowntime { get; set; }
        public string ReturnInformation { get; set; }
        public string ReturnShippingAccountNo { get; set; }
        public string PlannedReturnDate { get; set; }
        public string ActualReturnDate { get; set; }
        public string MajorAdoptionNumber { get; set; }
        public string OrderCreationDate { get; set; }
        public string ShippingId { get; set; }

        /*end of fields*/
    }
}
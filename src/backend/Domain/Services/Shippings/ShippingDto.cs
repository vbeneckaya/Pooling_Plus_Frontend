using System;

namespace Domain.Services.Shippings
{
    public class ShippingDto : IDto
    {
        public string Id { get; set; }
        public string ShippingNumber { get; set; }
        public string DeliveryType { get; set; }
        public int? TemperatureMin { get; set; }
        public int? TemperatureMax { get; set; }
        public string TarifficationType { get; set; }
        public string CarrierId { get; set; }
        public string VehicleTypeId { get; set; }
        public int? PalletsCount { get; set; }
        public int? ActualPalletsCount { get; set; }
        public int? ConfirmedPalletsCount { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? ActualWeightKg { get; set; }
        public string PlannedArrivalTimeSlotBDFWarehouse { get; set; }
        public string LoadingArrivalTime { get; set; }
        public string LoadingDepartureTime { get; set; }
        public string DeliveryInvoiceNumber { get; set; }
        public string DeviationReasonsComments { get; set; }
        public decimal? TotalDeliveryCost { get; set; }
        public decimal? OtherCosts { get; set; }
        public decimal? DeliveryCostWithoutVAT { get; set; }
        public decimal? ReturnCostWithoutVAT { get; set; }
        public decimal? InvoiceAmountWithoutVAT { get; set; }
        public decimal? AdditionalCostsWithoutVAT { get; set; }
        public string AdditionalCostsComments { get; set; }
        public decimal? TrucksDowntime { get; set; }
        public decimal? ReturnRate { get; set; }
        public decimal? AdditionalPointRate { get; set; }
        public decimal? DowntimeRate { get; set; }
        public decimal? BlankArrivalRate { get; set; }
        public bool? BlankArrival { get; set; }
        public bool? Waybill { get; set; }
        public bool? WaybillTorg12 { get; set; }
        public bool? TransportWaybill { get; set; }
        public bool? Invoice { get; set; }
        public string DocumentsReturnDate { get; set; }
        public string ActualDocumentsReturnDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Status { get; set; }
        public bool? CostsConfirmedByShipper { get; set; }
        public bool? CostsConfirmedByCarrier { get; set; }
        public string ShippingCreationDate { get; set; }
        /*end of fields*/
    }
}
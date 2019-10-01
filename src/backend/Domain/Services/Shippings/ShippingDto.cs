using System;

namespace Domain.Services.Shippings
{
    public class ShippingDto : IDto
    {
        public string Id { get; set; }
        public int? TransportationNumber { get; set; }
        public string DeliveryMethod { get; set; }
        public string ThermalMode { get; set; }
        public string BillingMethod { get; set; }
        public string TransportCompany { get; set; }
        public int? PalletsCount { get; set; }
        public int? ActualPalletsCount { get; set; }
        public string ConfirmedNumberOfPallets { get; set; }
        public string PlannedArrivalTimeSlotBDFWarehouse { get; set; }
        public DateTime? ArrivalTimeForLoadingBDFWarehouse { get; set; }
        public DateTime? DepartureTimeFromTheBDFWarehouse { get; set; }
        public string DeliveryInvoiceNumber { get; set; }
        public string CommentsReasonsForDeviationFromTheSchedule { get; set; }
        public string TransportationCostWithoutVAT { get; set; }
        public string ReturnShippingCostExcludingVAT { get; set; }
        public string AdditionalShippingCostsExcludingVAT { get; set; }
        public string AdditionalShippingCostsComments { get; set; }
        public string Waybill { get; set; }
        public string WaybillTorg12 { get; set; }
        public string WaybillTransportSection { get; set; }
        public string Invoice { get; set; }
        public string ActualReturnDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Status { get; set; }
        public string DeliveryStatus { get; set; }
        public string AmountConfirmedByShipper { get; set; }
        public string AmountConfirmedByTC { get; set; }
        /*end of fields*/
    }
}
using System;

namespace Domain.Persistables
{
    public class Shipping : IPersistable
    {
        public Guid Id { get; set; }
        public string TransportationNumber { get; set; }
        public string DeliveryMethod { get; set; }
        public string ThermalMode { get; set; }
        public string BillingMethod { get; set; }
        public string TransportCompany { get; set; }
        public string PreliminaryNumberOfPallets { get; set; }
        public string ActualNumberOfPallets { get; set; }
        public string ConfirmedNumberOfPallets { get; set; }
        public string PlannedArrivalTimeSlotBDFWarehouse { get; set; }
        public string ArrivalTimeForLoadingBDFWarehouse { get; set; }
        public string DepartureTimeFromTheBDFWarehouse { get; set; }
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
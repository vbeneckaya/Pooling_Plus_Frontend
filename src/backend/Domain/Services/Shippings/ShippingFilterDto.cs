using Domain.Shared.FormFilters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Shippings
{
    public class ShippingFilterDto: SearchFilterDto
    {
        public string ShippingNumber { get; set; }

        public string CarrierId { get; set; }
        
        public string ProviderId { get; set; }

        public string DeliveryType { get; set; }

        public string TarifficationType { get; set; }

        public string ShippingCreationDate { get; set; }

        public string Status { get; set; }

        public string TemperatureMin { get; set; }

        public string TemperatureMax { get; set; }

        public string VehicleTypeId { get; set; }

        public string BodyTypeId { get; set; }

        public string PalletsCount { get; set; }

        public string ConfirmedPalletsCount { get; set; }

        public string WeightKg { get; set; }

        public string ConfirmedWeightKg { get; set; }

        public string LoadingArrivalTime { get; set; }

        public string LoadingDepartureTime { get; set; }

        public string DeliveryInvoiceNumber { get; set; }

        public string DeviationReasonsComments { get; set; }

        public string TotalDeliveryCost { get; set; }

        public string OtherCosts { get; set; }

        public string DeliveryCostWithoutVAT { get; set; }

        public string ReturnCostWithoutVAT { get; set; }

        public string invoiceAmount { get; set; }
        
        public string AdditionalCostsWithoutVAT { get; set; }

        public string AdditionalCostsComments { get; set; }

        public string TrucksDowntime { get; set; }

        public string ReturnRate { get; set; }

        public string AdditionalPointRate { get; set; }

        public string DowntimeRate { get; set; }

        public string BlankArrivalRate { get; set; }

        public string BlankArrival { get; set; }

        public string Waybill { get; set; }

        public string WaybillTorg12 { get; set; }

        public string TransportWaybill { get; set; }

        public string Invoice { get; set; }

        public string DocumentsReturnDate { get; set; }

        public string ActualDocumentsReturnDate { get; set; }

        public string InvoiceNumber { get; set; }

        public string CostsConfirmedByShipper { get; set; }

        public string CostsConfirmedByCarrier { get; set; }
        
        public string Driver { get; set; }
        
        public string VehicleNumber { get; set; }
    }
}

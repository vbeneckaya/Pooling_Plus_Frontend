using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.Shippings
{
    public class ShippingDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Link), IsDefault, OrderNumber(1)]
        public string ShippingNumber { get; set; }

        [FieldType(FieldType.Enum, source: nameof(Enums.DeliveryType)), IsDefault, OrderNumber(4)]
        public string DeliveryType { get; set; }

        [FieldType(FieldType.Number)]
        public int? TemperatureMin { get; set; }

        [FieldType(FieldType.Number)]
        public int? TemperatureMax { get; set; }

        [FieldType(FieldType.Enum, source: nameof(Enums.TarifficationType)), IsDefault, OrderNumber(5)]
        public string TarifficationType { get; set; }

        [FieldType(FieldType.Select, source: nameof(TransportCompanies)), IsDefault, OrderNumber(3)]
        public string CarrierId { get; set; }

        [FieldType(FieldType.Select, source: nameof(VehicleTypes))]
        public string VehicleTypeId { get; set; }

        [FieldType(FieldType.Number)]
        public int? PalletsCount { get; set; }

        [FieldType(FieldType.Number)]
        public int? ActualPalletsCount { get; set; }

        [FieldType(FieldType.Number)]
        public int? ConfirmedPalletsCount { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? WeightKg { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? ActualWeightKg { get; set; }

        public string PlannedArrivalTimeSlotBDFWarehouse { get; set; }

        [FieldType(FieldType.DateTime)]
        public string LoadingArrivalTime { get; set; }

        [FieldType(FieldType.DateTime)]
        public string LoadingDepartureTime { get; set; }

        [FieldType(FieldType.Text)]
        public string DeliveryInvoiceNumber { get; set; }

        [FieldType(FieldType.Text)]
        public string DeviationReasonsComments { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? TotalDeliveryCost { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? OtherCosts { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? DeliveryCostWithoutVAT { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? ReturnCostWithoutVAT { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? InvoiceAmountWithoutVAT { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? AdditionalCostsWithoutVAT { get; set; }

        [FieldType(FieldType.Text)]
        public string AdditionalCostsComments { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? TrucksDowntime { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? ReturnRate { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? AdditionalPointRate { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? DowntimeRate { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? BlankArrivalRate { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? BlankArrival { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? Waybill { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? WaybillTorg12 { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? TransportWaybill { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? Invoice { get; set; }

        [FieldType(FieldType.Date)]
        public string DocumentsReturnDate { get; set; }

        [FieldType(FieldType.Date)]
        public string ActualDocumentsReturnDate { get; set; }

        [FieldType(FieldType.Text)]
        public string InvoiceNumber { get; set; }

        [FieldType(FieldType.State, source: nameof(ShippingState)), IsDefault, OrderNumber(2)]
        public string Status { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? CostsConfirmedByShipper { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? CostsConfirmedByCarrier { get; set; }

        [FieldType(FieldType.DateTime), IsDefault, OrderNumber(6), IgnoreFieldSettings]
        public string ShippingCreationDate { get; set; }
        /*end of fields*/
    }
}
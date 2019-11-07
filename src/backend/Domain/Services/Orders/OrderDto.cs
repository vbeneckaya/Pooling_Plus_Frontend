using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.Orders
{
    public class OrderDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.State, source: nameof(OrderState)), IsDefault, OrderNumber(2)]
        public string Status { get; set; }

        [FieldType(FieldType.Link), IsDefault, OrderNumber(1)]
        public string OrderNumber { get; set; }

        [FieldType(FieldType.Text)]
        public string ClientOrderNumber { get; set; }

        [FieldType(FieldType.Date)]
        public string OrderDate { get; set; }

        [FieldType(FieldType.Enum, source: nameof(Enums.OrderType))]
        public string OrderType { get; set; }

        [FieldType(FieldType.Text), IsDefault, OrderNumber(6)]
        public string Payer { get; set; }

        [FieldType(FieldType.Text), IsDefault, OrderNumber(5)]
        public string ClientName { get; set; }

        [FieldType(FieldType.Select, source: nameof(SoldTo), showRawValue: true)]
        public string SoldTo { get; set; }

        [FieldType(FieldType.Number)]
        public int? TemperatureMin { get; set; }

        [FieldType(FieldType.Number)]
        public int? TemperatureMax { get; set; }

        [FieldType(FieldType.Date), AllowBulkUpdate]
        public string ShippingDate { get; set; }

        [FieldType(FieldType.Number)]
        public int? TransitDays { get; set; }

        [FieldType(FieldType.Date), IsDefault, OrderNumber(7), AllowBulkUpdate]
        public string DeliveryDate { get; set; }

        [FieldType(FieldType.Number)]
        public int? ArticlesCount { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? BoxesCount { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? ConfirmedBoxesCount { get; set; }

        [FieldType(FieldType.Number)]
        public int? PalletsCount { get; set; }

        public bool? ManualPalletsCount { get; set; }

        [FieldType(FieldType.Number)]
        public int? ConfirmedPalletsCount { get; set; }

        [FieldType(FieldType.Number)]
        public int? ActualPalletsCount { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? WeightKg { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? ActualWeightKg { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? OrderAmountExcludingVAT { get; set; }

        public decimal? InvoiceAmountExcludingVAT { get; set; }

        [FieldType(FieldType.Text)]
        public string DeliveryRegion { get; set; }

        [FieldType(FieldType.Text)]
        public string DeliveryCity { get; set; }

        [FieldType(FieldType.BigText)]
        public string ShippingAddress { get; set; }

        [FieldType(FieldType.BigText)]
        public string DeliveryAddress { get; set; }

        [FieldType(FieldType.State, source: nameof(VehicleState))]
        public string ShippingStatus { get; set; }

        [FieldType(FieldType.State, source: nameof(VehicleState))]
        public string DeliveryStatus { get; set; }

        [FieldType(FieldType.Time)]
        public string ClientAvisationTime { get; set; }

        [FieldType(FieldType.Text)]
        public string OrderComments { get; set; }

        [FieldType(FieldType.Select, source: nameof(PickingTypes))]
        public string PickingTypeId { get; set; }

        public string PlannedArrivalTimeSlotBDFWarehouse { get; set; }

        [FieldType(FieldType.DateTime)]
        public string LoadingArrivalTime { get; set; }

        [FieldType(FieldType.DateTime)]
        public string LoadingDepartureTime { get; set; }

        [FieldType(FieldType.Date)]
        public string UnloadingArrivalDate { get; set; }

        [FieldType(FieldType.Time)]
        public string UnloadingArrivalTime { get; set; }

        [FieldType(FieldType.Date)]
        public string UnloadingDepartureDate { get; set; }

        [FieldType(FieldType.Time)]
        public string UnloadingDepartureTime { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? TrucksDowntime { get; set; }

        [FieldType(FieldType.Text)]
        public string ReturnInformation { get; set; }

        [FieldType(FieldType.Text)]
        public string ReturnShippingAccountNo { get; set; }

        [FieldType(FieldType.Date)]
        public string PlannedReturnDate { get; set; }

        [FieldType(FieldType.Date)]
        public string ActualReturnDate { get; set; }

        [FieldType(FieldType.Text)]
        public string MajorAdoptionNumber { get; set; }

        [FieldType(FieldType.DateTime), IsDefault, OrderNumber(8), IgnoreFieldSettings]
        public string OrderCreationDate { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? WaybillTorg12 { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? Invoice { get; set; }

        [FieldType(FieldType.Date)]
        public string DocumentsReturnDate { get; set; }

        [FieldType(FieldType.Date)]
        public string ActualDocumentsReturnDate { get; set; }

        public string ShippingId { get; set; }

        [FieldType(FieldType.Text), IsDefault, OrderNumber(3)]
        public string ShippingNumber { get; set; }

        [FieldType(FieldType.State, source: nameof(ShippingState)), IsDefault, OrderNumber(4)]
        public string OrderShippingStatus { get; set; }

        public bool? IsActive { get; set; }

        public string AdditionalInfo { get; set; }

        public string ShippingWarehouseId { get; set; }

        [FieldType(FieldType.DateTime), IgnoreFieldSettings]
        public string OrderChangeDate { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? OrderConfirmed { get; set; }

        /*end of fields*/
    }
}
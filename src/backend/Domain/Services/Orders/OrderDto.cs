using System;
using Application.Shared.Excel.Columns;
using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.Orders
{
    public class OrderDto : IDto
    {
        [ExcelIgnore]
        public string Id { get; set; }

        [FieldType(FieldType.State, source: nameof(OrderState)), IsDefault, OrderNumber(2), IsReadOnly]
        public string Status { get; set; }

        [FieldType(FieldType.Link), IsDefault, OrderNumber(1), IsReadOnly, IsRequired]
        public string OrderNumber { get; set; }

        [FieldType(FieldType.Text), IsRequired]
        public string ClientOrderNumber { get; set; }

        [FieldType(FieldType.Date), IsRequired]
        public string OrderDate { get; set; }

        [FieldType(FieldType.Enum, source: nameof(Enums.OrderType)), IsReadOnly]
        public string OrderType { get; set; }

        [FieldType(FieldType.Text), IsDefault, OrderNumber(6)]
        public string Payer { get; set; }

        [FieldType(FieldType.Text), IsDefault, OrderNumber(5), IsReadOnly]
        public string ClientName { get; set; }

        [FieldType(FieldType.Select, source: nameof(SoldTo), showRawValue: true), IsRequired]
        public LookUpDto SoldTo { get; set; }

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

        [FieldType(FieldType.Select, source: nameof(ShippingWarehouseCity), showRawValue: true), IsReadOnly]
        public LookUpDto ShippingCity { get; set; }

        [FieldType(FieldType.Text), IsReadOnly]
        public string DeliveryRegion { get; set; }

        [FieldType(FieldType.Select, source: nameof(WarehouseCity), showRawValue: true), IsReadOnly]
        public LookUpDto DeliveryCity { get; set; }

        [FieldType(FieldType.BigText), IsReadOnly]
        public string ShippingAddress { get; set; }

        [FieldType(FieldType.BigText), IsReadOnly]
        public string DeliveryAddress { get; set; }

        [FieldType(FieldType.State, source: nameof(VehicleState)), IsReadOnly]
        public string ShippingStatus { get; set; }

        [FieldType(FieldType.State, source: nameof(VehicleState))]
        public string DeliveryStatus { get; set; }

        [FieldType(FieldType.Time)]
        public string ShippingAvisationTime { get; set; }

        [FieldType(FieldType.Time)]
        public string ClientAvisationTime { get; set; }

        [FieldType(FieldType.Text)]
        public string OrderComments { get; set; }

        [FieldType(FieldType.Select, source: nameof(PickingTypes))]
        public LookUpDto PickingTypeId { get; set; }

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

        [FieldType(FieldType.LocalDateTime), IsDefault, OrderNumber(8), IsReadOnly]
        public DateTime? OrderCreationDate { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? WaybillTorg12 { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? Invoice { get; set; }

        [FieldType(FieldType.Date)]
        public string DocumentsReturnDate { get; set; }

        [FieldType(FieldType.Date)]
        public string ActualDocumentsReturnDate { get; set; }

        public string ShippingId { get; set; }

        [FieldType(FieldType.Text), IsDefault, OrderNumber(3), IsReadOnly]
        public string ShippingNumber { get; set; }

        [FieldType(FieldType.State, source: nameof(ShippingState)), IsDefault, OrderNumber(4)]
        public string OrderShippingStatus { get; set; }

        public bool? IsActive { get; set; }

        public string AdditionalInfo { get; set; }

        [FieldType(FieldType.Select, source: "ShippingWarehousesForOrderCreation")]
        public LookUpDto ShippingWarehouseId { get; set; }

        [FieldType(FieldType.LocalDateTime), IsReadOnly]
        public DateTime? OrderChangeDate { get; set; }

        [FieldType(FieldType.Boolean), AllowBulkUpdate]
        public bool? OrderConfirmed { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? DocumentReturnStatus { get; set; }

        [FieldType(FieldType.Text), IsReadOnly]
        public string PickingFeatures { get; set; }

        [FieldType(FieldType.Select, source: nameof(TransportCompanies)), IsReadOnly]
        public LookUpDto CarrierId { get; set; }

        [FieldType(FieldType.Enum, source: nameof(Enums.DeliveryType))]
        public string DeliveryType { get; set; }

        [FieldType(FieldType.BigText)]
        public string DeviationsComment { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? DeliveryCost { get; set; }

        public bool? ManualDeliveryCost { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? ActualDeliveryCost { get; set; }

        public string Source { get; set; }
    }
}
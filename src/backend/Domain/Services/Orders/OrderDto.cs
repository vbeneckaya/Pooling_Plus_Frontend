using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;
using System;

namespace Domain.Services.Orders
{
    public class OrderDto : IDto
    {
        public string Id { get; set; }

        [DisplayNameKey("Order.Status")]
        [FieldType(FieldType.State, source: nameof(OrderState)), IsDefault, OrderNumber(4), IsReadOnly]
        public string Status { get; set; }

        [FieldType(FieldType.Link), IsDefault, OrderNumber(2), IsReadOnly, IsRequired]
        public string OrderNumber { get; set; }

        [FieldType(FieldType.Text)]
        public string ClientOrderNumber { get; set; }

        [FieldType(FieldType.Date), IsDefault, OrderNumber(3), IsRequired]
        public string OrderDate { get; set; }

        [FieldType(FieldType.Enum, source: nameof(Enums.OrderType)), IsReadOnly]
        public LookUpDto OrderType { get; set; }

        [FieldType(FieldType.Text)]
        public string Payer { get; set; }

        [FieldType(FieldType.Select, source: nameof(Clients)), IsDefault, OrderNumber(13), IsRequired]
        public LookUpDto ClientId { get; set; }

        [FieldType(FieldType.Number)]
        public int? TemperatureMin { get; set; }

        [FieldType(FieldType.Number)]
        public int? TemperatureMax { get; set; }

        [FieldType(FieldType.Date), AllowBulkUpdate, IsDefault, OrderNumber(5)]
        public string ShippingDate { get; set; }

        [FieldType(FieldType.Number)]
        public int? TransitDays { get; set; }

        [FieldType(FieldType.Date), IsDefault, OrderNumber(6), IsRequired, AllowBulkUpdate]
        public string DeliveryDate { get; set; }

        [FieldType(FieldType.Number)]
        public int? ArticlesCount { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? BoxesCount { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? ConfirmedBoxesCount { get; set; }

        [FieldType(FieldType.Number), IsDefault, OrderNumber(11)]
        public int? PalletsCount { get; set; }

        public bool? ManualPalletsCount { get; set; }

        [FieldType(FieldType.Number)]
        public int? ConfirmedPalletsCount { get; set; }

        [FieldType(FieldType.Number)]
        public int? ActualPalletsCount { get; set; }

        [FieldType(FieldType.Number), IsDefault, OrderNumber(12)]
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

        public string ShippingAddress { get; set; }

        public string DeliveryAddress { get; set; }

        [FieldType(FieldType.State, source: nameof(VehicleState))]
        public string ShippingStatus { get; set; }

        [FieldType(FieldType.State, source: nameof(VehicleState))]
        public string DeliveryStatus { get; set; }

        [FieldType(FieldType.Time), AllowBulkUpdate]
        public string ShippingAvisationTime { get; set; }

        [FieldType(FieldType.Time), AllowBulkUpdate]
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

        [FieldType(FieldType.LocalDateTime), IsDefault, OrderNumber(1), IsReadOnly]
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

        [FieldType(FieldType.Text), IsDefault, OrderNumber(9), IsReadOnly]
        public string ShippingNumber { get; set; }

        [FieldType(FieldType.State, source: nameof(OrderShippingStatus)), IsDefault, OrderNumber(10), IsReadOnly]
        public string OrderShippingStatus { get; set; }

        public bool? IsActive { get; set; }

        public string AdditionalInfo { get; set; }

        [FieldType(FieldType.Select, source: nameof(ShippingAddress)), IsDefault, OrderNumber(7), IsRequired]
        public LookUpDto ShippingWarehouseId { get; set; }

        [FieldType(FieldType.Select, source: nameof(DeliveryAddress)), IsDefault, OrderNumber(8), IsRequired]
        public LookUpDto DeliveryWarehouseId { get; set; }

        [FieldType(FieldType.LocalDateTime), IsReadOnly]
        public DateTime? OrderChangeDate { get; set; }

        [FieldType(FieldType.Boolean), AllowBulkUpdate]
        public bool? OrderConfirmed { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? DocumentReturnStatus { get; set; }

        [FieldType(FieldType.Text), IsReadOnly]
        public string PickingFeatures { get; set; }

        [FieldType(FieldType.Select, source: nameof(TransportCompanies)), AllowBulkUpdate]
        public LookUpDto CarrierId { get; set; }

        [FieldType(FieldType.Enum, source: nameof(Enums.DeliveryType)), IsDefault, OrderNumber(14)]
        public LookUpDto DeliveryType { get; set; }

        [FieldType(FieldType.BigText)]
        public string DeviationsComment { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? DeliveryCost { get; set; }

        public bool? ManualDeliveryCost { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? ActualDeliveryCost { get; set; }

        public string Source { get; set; }
        
        [FieldType(FieldType.Enum, source: nameof(TarifficationType))]
        public LookUpDto TarifficationType { get; set; }
        
        [FieldType(FieldType.Select, source: nameof(VehicleTypes))]
        public LookUpDto VehicleTypeId { get; set; }

        [FieldType(FieldType.Select, source: nameof(Companies))]
        public LookUpDto CompanyId { get; set; }

        [FieldType(FieldType.Select, source: nameof(ProductTypes))]
        public LookUpDto ProductTypeId { get; set; }

        [FieldType(FieldType.Number)]
        public int? ItemsNumber { get; set; }

        public string ShippingWarehouseGln { get; set; }

        public string DeliveryWarehouseGln { get; set; }

        public string ShippingRegion { get; set; }

        public bool IsEditable { get; set; }
        
        [FieldType(FieldType.Text)]
        public string Driver { get; set; }
        
        [FieldType(FieldType.Text)]
        public string VehicleNumber { get; set; }
    }
}
using System;
using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.Orders
{
    //TODO Собирать поля ленивым рефлекшеном(сохранять как кеш после первого обращения)
    public class OrderDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.State, source: nameof(OrderState)), IsDefault(2)]
        public string Status { get; set; }

        [FieldType(FieldType.Link), IsDefault(1)]
        public string OrderNumber { get; set; }

        [FieldType(FieldType.DateTime)]
        public string OrderDate { get; set; }

        [FieldType(FieldType.Enum, source: nameof(OrderType))]
        public string OrderType { get; set; }

        [FieldType(FieldType.Text), IsDefault(6)]
        public string Payer { get; set; }

        [FieldType(FieldType.Text), IsDefault(5)]
        public string ClientName { get; set; }

        [FieldType(FieldType.Select, source: nameof(SoldTo), showRawValue: true)]
        public string SoldTo { get; set; }

        [FieldType(FieldType.Number)]
        public int? TemperatureMin { get; set; }

        [FieldType(FieldType.Number)]
        public int? TemperatureMax { get; set; }

        [FieldType(FieldType.DateTime)]
        public string ShippingDate { get; set; }

        [FieldType(FieldType.Number)]
        public int? TransitDays { get; set; }

        [FieldType(FieldType.DateTime), IsDefault(7)]
        public string DeliveryDate { get; set; }

        [FieldType(FieldType.Text)]
        public string BdfInvoiceNumber { get; set; }

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

        [FieldType(FieldType.Text)]
        public string ShippingAddress { get; set; }

        [FieldType(FieldType.Text)]
        public string DeliveryAddress { get; set; }

        [FieldType(FieldType.State, source: nameof(VehicleState))]
        public string ShippingStatus { get; set; }

        [FieldType(FieldType.State, source: nameof(VehicleState))]
        public string DeliveryStatus { get; set; }

        [FieldType(FieldType.Time)]
        public string ClientAvisationTime { get; set; }

        [FieldType(FieldType.Text)]
        public string OrderComments { get; set; }

        [FieldType(FieldType.Select, source: "PickingTypes")]
        public string PickingTypeId { get; set; }

        public string PlannedArrivalTimeSlotBDFWarehouse { get; set; }

        [FieldType(FieldType.DateTime)]
        public string LoadingArrivalTime { get; set; }

        [FieldType(FieldType.DateTime)]
        public string LoadingDepartureTime { get; set; }

        [FieldType(FieldType.DateTime)]
        public string UnloadingArrivalDate { get; set; }

        [FieldType(FieldType.Time)]
        public string UnloadingArrivalTime { get; set; }

        [FieldType(FieldType.DateTime)]
        public string UnloadingDepartureDate { get; set; }

        [FieldType(FieldType.Time)]
        public string UnloadingDepartureTime { get; set; }

        [FieldType(FieldType.Number)]
        public decimal? TrucksDowntime { get; set; }

        [FieldType(FieldType.Text)]
        public string ReturnInformation { get; set; }

        [FieldType(FieldType.Text)]
        public string ReturnShippingAccountNo { get; set; }

        [FieldType(FieldType.DateTime)]
        public string PlannedReturnDate { get; set; }

        [FieldType(FieldType.DateTime)]
        public string ActualReturnDate { get; set; }

        [FieldType(FieldType.Text)]
        public string MajorAdoptionNumber { get; set; }

        [FieldType(FieldType.DateTime), IsDefault(8)]
        public string OrderCreationDate { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? WaybillTorg12 { get; set; }

        [FieldType(FieldType.Boolean)]
        public bool? Invoice { get; set; }

        [FieldType(FieldType.DateTime)]
        public string DocumentsReturnDate { get; set; }

        [FieldType(FieldType.DateTime)]
        public string ActualDocumentsReturnDate { get; set; }

        public string ShippingId { get; set; }

        [FieldType(FieldType.Text), IsDefault(3)]
        public string ShippingNumber { get; set; }

        [FieldType(FieldType.State, source: nameof(ShippingState)), IsDefault(4)]
        public string OrderShippingStatus { get; set; }

        public bool? IsActive { get; set; }

        public string AdditionalInfo { get; set; }

        /*end of fields*/
    }
}
using Application.Shared.Excel.Columns;
using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.Warehouses
{
    public class WarehouseDto : IDto
    {
        [ExcelIgnore]
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string WarehouseName { get; set; }

        [FieldType(FieldType.Text), OrderNumber(2), IsRequired]
        public string SoldToNumber { get; set; }

        [ExcelIgnore]
        public string PostalCode { get; set; }

        [FieldType(FieldType.Text), OrderNumber(3), IsReadOnly]
        public string Region { get; set; }

        [ExcelIgnore]
        public string Area { get; set; }

        [FieldType(FieldType.Text), OrderNumber(4), IsReadOnly]
        public string City { get; set; }

        [ExcelIgnore]
        public string Street { get; set; }

        [ExcelIgnore]
        public string House { get; set; }

        [FieldType(FieldType.Text), OrderNumber(5)]
        public string Address { get; set; }

        [ExcelIgnore]
        public string ValidAddress { get; set; }

        [ExcelIgnore]
        public string UnparsedAddressParts { get; set; }

        [FieldType(FieldType.Select, source: nameof(PickingTypes)), OrderNumber(6)]
        public LookUpDto PickingTypeId { get; set; }

        [FieldType(FieldType.Text), OrderNumber(7)]
        public string PickingFeatures { get; set; }

        [FieldType(FieldType.Number), OrderNumber(8)]
        public int? LeadtimeDays { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(9)]
        public bool CustomerWarehouse { get; set; }

        [FieldType(FieldType.Enum, source: nameof(Enums.DeliveryType)), IsRequired, OrderNumber(10)]
        public LookUpDto DeliveryType { get; set; }

        [FieldType(FieldType.Time), OrderNumber(10)]
        public string AvisaleTime { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(11)]
        public bool? IsActive { get; set; }
    }
}
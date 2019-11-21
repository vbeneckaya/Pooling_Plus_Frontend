using Application.Shared.Excel.Columns;
using Domain.Enums;
using Domain.Extensions;

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

        public string PostalCode { get; set; }

        [FieldType(FieldType.Text), OrderNumber(3)]
        public string Region { get; set; }

        public string Area { get; set; }

        [FieldType(FieldType.Text), OrderNumber(4)]
        public string City { get; set; }

        public string Street { get; set; }

        public string House { get; set; }

        [FieldType(FieldType.Text), OrderNumber(5)]
        public string Address { get; set; }

        public string ValidAddress { get; set; }

        public string UnparsedAddressParts { get; set; }

        [FieldType(FieldType.Select, source: nameof(PickingTypes)), OrderNumber(6)]
        public string PickingTypeId { get; set; }

        [FieldType(FieldType.Text), OrderNumber(7)]
        public string PickingFeatures { get; set; }

        [FieldType(FieldType.Number), OrderNumber(8)]
        public int? LeadtimeDays { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(9)]
        public bool CustomerWarehouse { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(10)]
        public bool? IsActive { get; set; }
    }
}
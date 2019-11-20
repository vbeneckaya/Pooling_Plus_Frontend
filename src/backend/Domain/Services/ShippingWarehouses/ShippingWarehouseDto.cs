using Application.Shared.Excel.Columns;
using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.ShippingWarehouses
{
    public class ShippingWarehouseDto : IDto
    {
        [ExcelIgnore]
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Code { get; set; }

        [FieldType(FieldType.Text), OrderNumber(2), IsRequired]
        public string WarehouseName { get; set; }

        [FieldType(FieldType.Text), OrderNumber(3)]
        public string Address { get; set; }

        [ExcelIgnore]
        public string ValidAddress { get; set; }

        [ExcelIgnore]
        public string PostalCode { get; set; }

        [ExcelIgnore]
        public string Region { get; set; }

        [ExcelIgnore]
        public string Area { get; set; }

        [ExcelIgnore]
        [FieldType(FieldType.Text), OrderNumber(4)]
        public string City { get; set; }

        [ExcelIgnore]
        public string Street { get; set; }

        [ExcelIgnore]
        public string House { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(4)]
        public bool? IsActive { get; set; }
    }
}

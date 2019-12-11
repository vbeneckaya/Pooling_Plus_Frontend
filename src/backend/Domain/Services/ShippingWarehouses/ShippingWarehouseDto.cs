using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.ShippingWarehouses
{
    public class ShippingWarehouseDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Code { get; set; }

        [FieldType(FieldType.Text), OrderNumber(2), IsRequired]
        public string WarehouseName { get; set; }

        [FieldType(FieldType.Text), OrderNumber(5)]
        public string Address { get; set; }

        public string ValidAddress { get; set; }

        public string PostalCode { get; set; }

        [FieldType(FieldType.Text), OrderNumber(3), IsReadOnly]
        public string Region { get; set; }

        public string Area { get; set; }

        [FieldType(FieldType.Text), OrderNumber(4), IsReadOnly]
        public string City { get; set; }

        public string Street { get; set; }

        public string House { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(6)]
        public bool? IsActive { get; set; }
    }
}

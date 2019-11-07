using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.ShippingWarehouses
{
    public class ShippingWarehouseDto : IDto
    { 
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1)]
        public string Code { get; set; }

        [FieldType(FieldType.Text), OrderNumber(2)]
        public string WarehouseName { get; set; }

        [FieldType(FieldType.Text), OrderNumber(3)]
        public string Address { get; set; }

        public string ValidAddress { get; set; }

        public string PostalCode { get; set; }

        public string Region { get; set; }

        public string Area { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string House { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(4)]
        public bool? IsActive { get; set; }
    }
}

using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.ShippingWarehouses
{
    public class ShippingWarehouseDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(5)]
        public string Gln { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string WarehouseName { get; set; }

        [FieldType(FieldType.Text), OrderNumber(2), IsRequired]
        public string Address { get; set; }

        public string PostalCode { get; set; }

        [FieldType(FieldType.Text), OrderNumber(3)]
        public string Region { get; set; }

        public string Area { get; set; }

        public string Settlement { get; set; }

        [FieldType(FieldType.Text), OrderNumber(4)]
        public string City { get; set; }

        public string Street { get; set; }

        public string House { get; set; }

        public string Block { get; set; }

        public string UnparsedParts { get; set; }

        public string RegionId { get; set; }

        [FieldType(FieldType.Select, source: nameof(Clients)), OrderNumber(6)]
        public LookUpDto ClientId { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(7)]
        public bool? IsActive { get; set; }

        public bool IsEditable { get; set; }
    }
}

using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.Warehouses
{
    public class WarehouseDto : IDto
    {
        public string Id { get; set; }

        [DisplayNameKey("Warehouse.WarehouseName")]
        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string WarehouseName { get; set; }

        //public string SoldToNumber { get; set; }

        public string PostalCode { get; set; }

        [FieldType(FieldType.Text), OrderNumber(3), IsReadOnly]
        public string Region { get; set; }

        public string Area { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string House { get; set; }

        [FieldType(FieldType.Text), OrderNumber(5)]
        public string Address { get; set; }
        public string ValidAddress { get; set; }

        public string UnparsedAddressParts { get; set; }

        [FieldType(FieldType.Select, source: nameof(PickingTypes)), OrderNumber(6)]
        public LookUpDto PickingTypeId { get; set; }

        [FieldType(FieldType.Number), OrderNumber(8)]
        public int? LeadtimeDays { get; set; }

        [FieldType(FieldType.Enum, source: nameof(Enums.DeliveryType)), IsRequired, OrderNumber(10)]
        public LookUpDto DeliveryType { get; set; }

        [FieldType(FieldType.Time), OrderNumber(10)]
        public string AvisaleTime { get; set; }

        [FieldType(FieldType.Select, source: nameof(Companies)), OrderNumber(11)]
        public LookUpDto CompanyId { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(12)]
        public bool? IsActive { get; set; }

        public string AdditionalInfo { get; set; }

        [FieldType(FieldType.Select, source: nameof(Clients)), OrderNumber(11)]
        public LookUpDto ClientId { get; set; }

        [FieldType(FieldType.Text), OrderNumber(12)]
        public string Gln { get; set; }

        public bool IsEditable { get; set; }
    }
}
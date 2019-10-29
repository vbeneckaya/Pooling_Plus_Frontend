using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.Warehouses
{
    public class WarehouseDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1)]
        public string WarehouseName { get; set; }

        [FieldType(FieldType.Text), OrderNumber(2)]
        public string SoldToNumber { get; set; }

        [FieldType(FieldType.Text), OrderNumber(3)]
        public string Region { get; set; }

        [FieldType(FieldType.Text), OrderNumber(4)]
        public string City { get; set; }

        [FieldType(FieldType.Text), OrderNumber(5)]
        public string Address { get; set; }

        [FieldType(FieldType.Select, source: nameof(PickingTypes)), OrderNumber(6)]
        public string PickingTypeId { get; set; }

        [FieldType(FieldType.Number), OrderNumber(7)]
        public int? LeadtimeDays { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(8)]
        public bool CustomerWarehouse { get; set; }
        /*end of fields*/
    }
}
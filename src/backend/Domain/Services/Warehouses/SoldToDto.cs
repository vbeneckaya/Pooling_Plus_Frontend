using Domain.Shared;

namespace Domain.Services.Warehouses
{
    public class SoldToDto : LookUpDto
    {
        public string WarehouseName { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PickingTypeId { get; set; }
        public string LeadtimeDays { get; set; }
    }
}

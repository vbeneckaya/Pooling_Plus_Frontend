namespace Domain.Services.Warehouses
{
    public class WarehouseDto : IDto
    {
        public string Id { get; set; }
        public string WarehouseName { get; set; }
        public string SoldToNumber { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PickingTypeId { get; set; }
        public int? LeadtimeDays { get; set; }
        public bool CustomerWarehouse { get; set; }
        public bool UsePickingType { get; set; }
        /*end of fields*/
    }
}
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
        public string PickingType { get; set; }
        public string LeadtimeDays { get; set; }
        public string CustomerWarehouse { get; set; }
        public string UsePickingType { get; set; }
        /*end of fields*/
    }
}
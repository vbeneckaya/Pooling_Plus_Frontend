namespace Domain.Services.Warehouses
{
    public class WarehouseDto : IDto
    {
        public string Id { get; set; }
        public string TheNameOfTheWarehouse { get; set; }
        public string SoldToNumber { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string TypeOfEquipment { get; set; }
        public string LeadtimeDays { get; set; }
        public string CustomerWarehouse { get; set; }
        /*end of fields*/
    }
}
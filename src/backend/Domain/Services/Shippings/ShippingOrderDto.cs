using Domain.Shared;

namespace Domain.Services.Shippings
{
    public class ShippingOrderDto
    {
        public string Id { get; set; }
        
        public LookUpDto ClientId { get; set; }
        
        public LookUpDto OrderNumber { get; set; }
        
        public string ClientOrderNumber { get; set; }
        
        public int? PalletsCount { get; set; }
        
        public decimal? WeightKg { get; set; }
        
        public LookUpDto ShippingWarehouseId { get; set; }
        
        public string ShippingDate { get; set; }
        
        public LookUpDto DeliveryWarehouseId { get; set; }
        
        public string DeliveryDate { get; set; }
        
        public string Status { get; set; }
    }
}

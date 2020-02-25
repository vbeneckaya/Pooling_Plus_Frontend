using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.Shippings
{
    public class ShippingOrderDto
    {
        public string Id { get; set; }
        
        [FieldType(FieldType.Select, source: nameof(Clients))]
        public LookUpDto ClientId { get; set; }
        
        [FieldType(FieldType.Link, source: nameof(Orders))]
        public LookUpDto OrderNumber { get; set; }
        
        [FieldType(FieldType.Text)]
        public string ClientOrderNumber { get; set; }
        
        [FieldType(FieldType.Number)]
        public int? PalletsCount { get; set; }
        
        [FieldType(FieldType.Number)]
        public decimal? WeightKg { get; set; }
        
        [FieldType(FieldType.Select, source: nameof(ShippingWarehouses))]
        public LookUpDto ShippingWarehouseId { get; set; }
        
        public string ShippingDate { get; set; }
        
        [FieldType(FieldType.Select, source: nameof(Warehouses))]
        public LookUpDto DeliveryWarehouseId { get; set; }
        
        [FieldType(FieldType.Date)]
        public string DeliveryDate { get; set; }
        
        public string Status { get; set; }
    }
}

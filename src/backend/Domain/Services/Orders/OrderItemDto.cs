namespace Domain.Services.Orders
{
    public class OrderItemDto
    {
        public string Id { get; set; }
        public string Nart { get; set; }
        public string Description { get; set; }
        public string CountryOfOrigin { get; set; }
        public string SPGR { get; set; }
        public string Ean { get; set; }
        public int? ShelfLife { get; set; }
        public int? Quantity { get; set; }
    }
}

namespace Integrations.Pooling.Dtos
{
    public class GetSlotsParameters
    {
        //2020-02-10
        public string DateFrom { get; set; }
        //2020-02-16
        public string DateTo { get; set; }
        //Tent
        public string CarType { get; set; }
        //Alco
        public string ProductType { get; set; }
        //5c9345ffb4018629d4da42cd
        public string ClientId { get; set; }
        //5e21c2a29b2abb000186dc16
        public string CarrierId { get; set; }
        //5bf87204dc28430b4422a7f9
        public string ShippingRegionId { get; set; }
        public string WarehouseId { get; set; }
    }
}
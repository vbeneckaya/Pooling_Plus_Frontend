namespace Domain.Services.Orders
{
    public class OrderDto : IDto
    {
        public string Id { get; set; }
        public string Incoming { get; set; }
    }
}
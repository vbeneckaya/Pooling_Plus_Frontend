using System.Collections.Generic;

namespace Domain.Services.Orders
{
    public class OrderFormDto : OrderDto
    {
        public string ShippingNumber { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}

using System.Collections.Generic;

namespace Domain.Services.Orders
{
    public class OrderFormDto : OrderDto
    {
        public OrderFormDto()
        {
            Items = new List<OrderItemDto>();
        }
        public List<OrderItemDto> Items { get; set; }
    }
}

using System.Collections.Generic;

namespace Domain.Services.Shippings
{
    public class ShippingFormDto : ShippingDto
    {
        public List<RoutePointDto> RoutePoints { get; set; }
    }
}

using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Shippings;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/shippings")]
    public class ShippingsController : GridController<IShippingsService, Shipping, ShippingDto, ShippingDto> 
    {
        public ShippingsController(IShippingsService shippingsService) : base(shippingsService)
        {
        }
    }
}
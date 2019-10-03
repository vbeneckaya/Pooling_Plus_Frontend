using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/deliveryType")]
    public class DeliveryTypeController : EnumController<DeliveryType>
    {
        
    }
}
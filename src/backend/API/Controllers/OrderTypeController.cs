using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/orderType")]
    public class OrderTypeController : EnumController<OrderType>
    {
        
    }
}
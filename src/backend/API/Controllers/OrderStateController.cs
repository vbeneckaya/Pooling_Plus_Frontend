using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/orderState")]
    public class OrderStateController : StateController<OrderState>
    {
        
    }
}
using Domain.Enums;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/orderState")]
    public class OrderStateController : StateController<OrderState>
    {
        public OrderStateController(IStateService stateService) : base(stateService)
        {
        }
    }
}
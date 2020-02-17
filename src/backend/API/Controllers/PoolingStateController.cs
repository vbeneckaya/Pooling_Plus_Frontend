using Domain.Enums;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/shippingPoolingState")]
    public class PoolingStateController : StateController<ShippingPoolingState>
    {
        public PoolingStateController (IStateService stateService) : base(stateService)
        {
        }
    }
}
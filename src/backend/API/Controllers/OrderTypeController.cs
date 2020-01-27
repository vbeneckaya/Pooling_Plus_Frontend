using Domain.Enums;
using Domain.Services.OrderTypes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/orderType")]
    public class OrderTypeController : EnumController<IOrderTypesService,OrderType>
    {
        public OrderTypeController(IOrderTypesService service) : base(service)
        {
        }
    }
}
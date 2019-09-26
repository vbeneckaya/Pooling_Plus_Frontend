using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Orders;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/orders")]
    public class OrdersController : GridWithDocumentsController<IOrdersService, Order, OrderDto, OrderFormDto> 
    {
        public OrdersController(IOrdersService ordersService) : base(ordersService)
        {
        }
    }
}
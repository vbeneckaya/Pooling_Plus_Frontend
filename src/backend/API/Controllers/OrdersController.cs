using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Orders;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("Orders")]
    public class OrdersController : GridController<IOrdersService, Order, OrderDto> 
    {
        public OrdersController(IOrdersService ordersService) : base(ordersService)
        {
        }
    }
}
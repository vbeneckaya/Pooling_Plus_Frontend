using System.Collections.Generic;
using Domain.Persistables;
using Domain.Services.Orders;
using Domain.Services.Transportations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("Orders")]
    public class OrdersController : DictonaryController<IOrdersService, Order, OrderDto> 
    {
        public OrdersController(IOrdersService ordersService) : base(ordersService)
        {
        }
    }
}

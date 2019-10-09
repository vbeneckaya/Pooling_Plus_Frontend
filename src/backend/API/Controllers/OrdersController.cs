using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Orders;
using Domain.Shared;
using Domain.Shared.FormFilters;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/orders")]
    public class OrdersController : GridWithDocumentsController<IOrdersService, Order, OrderDto, OrderFormDto, OrderSummaryDto, FilterFormDto<OrderFilterDto>> 
    {
        public OrdersController(IOrdersService ordersService) : base(ordersService)
        {
        }
    }
}
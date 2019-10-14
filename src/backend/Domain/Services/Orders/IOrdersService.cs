using Domain.Persistables;
using Domain.Shared;
using Domain.Shared.FormFilters;

namespace Domain.Services.Orders
{
    public interface IOrdersService : IGridService<Order, OrderDto, OrderFormDto, OrderSummaryDto, FilterFormDto<OrderFilterDto>>
    {
    }
}
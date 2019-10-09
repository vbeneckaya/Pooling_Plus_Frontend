using Domain.Persistables;
using Domain.Shared;
using Domain.Shared.FormFilters;

namespace Domain.Services.Orders
{
    public interface IOrdersService : IGridWithDocuments<OrderDto, OrderFormDto, FilterFormDto<OrderFilterDto>>
    {
    }
}
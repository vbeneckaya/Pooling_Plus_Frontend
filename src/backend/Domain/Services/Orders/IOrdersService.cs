using Domain.Persistables;

namespace Domain.Services.Orders
{
    public interface IOrdersService : IGridWithDocuments<Order, OrderDto, OrderFormDto, OrderSummaryDto>
    {
    }
}
using Domain.Persistables;

namespace Domain.Services.Orders
{
    public interface IOrdersService : IGridWithDocuments<OrderDto, OrderFormDto>
    {
    }
}
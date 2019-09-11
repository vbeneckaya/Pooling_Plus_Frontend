using Domain.Persistables;

namespace Domain.Services.Orders
{
    public interface IOrdersService : IDictonaryService<Order, OrderDto>
    {
    }
}
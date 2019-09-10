using Domain.Persistables;
using Domain.Services.Users;

namespace Domain.Services.Orders
{
    public interface IOrdersService : IDictonaryService<Order, OrderDto>
    {
    }
}
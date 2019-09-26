using Domain.Persistables;

namespace Domain.Services.Orders
{
    public interface IOrdersService : IGridService<Order, OrderDto, OrderFormDto>
    {
    }
}
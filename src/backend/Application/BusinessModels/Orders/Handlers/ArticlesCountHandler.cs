using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ArticlesCountHandler : IFieldHandler<Order, int?>
    {
        public void AfterChange(Order order, int? oldValue, int? newValue)
        {
            order.OrderChangeDate = DateTime.UtcNow;
        }

        public string ValidateChange(Order order, int? oldValue, int? newValue)
        {
            return null;
        }
    }
}

using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using Domain.Services.History;

namespace Application.BusinessModels.Orders.Handlers
{
    public class OrderItemQuantityHandler : IFieldHandler<OrderItem, int>
    {
        public void AfterChange(OrderItem entity, int oldValue, int newValue)
        {
            if (!_isNew)
            {
                _historyService.Save(entity.OrderId, "orderItemChangeQuantity", entity.Nart, newValue);
            }
        }

        public string ValidateChange(OrderItem entity, int oldValue, int newValue)
        {
            return null;
        }

        public OrderItemQuantityHandler(IHistoryService historyService, bool isNew)
        {
            _historyService = historyService;
            _isNew = isNew;
        }

        private readonly IHistoryService _historyService;
        private readonly bool _isNew;
    }
}

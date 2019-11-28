using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;

namespace Application.BusinessModels.Orders.Handlers
{
    public class DeliveryCostHandler : IFieldHandler<Order, decimal?>
    {
        private readonly bool _isManual;

        public DeliveryCostHandler(bool isManual)
        {
            _isManual = isManual;
        }

        public void AfterChange(Order order, decimal? oldValue, decimal? newValue)
        {
            if (_isManual)
            {
                order.ManualDeliveryCost = true;
            }
        }

        public string ValidateChange(Order order, decimal? oldValue, decimal? newValue)
        {
            return null;
        }
    }
}

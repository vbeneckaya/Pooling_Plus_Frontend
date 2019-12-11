using Application.BusinessModels.Shared.Triggers;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Shared;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Triggers
{
    public class MakeOrderCreated : ITrigger<Order>
    {
        private readonly IHistoryService _historyService;

        public MakeOrderCreated(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        public void Execute(Order order)
        {
            bool hasRequiredFields =
                   !string.IsNullOrEmpty(order.SoldTo)
                && !string.IsNullOrEmpty(order.ShippingAddress)
                && !string.IsNullOrEmpty(order.DeliveryAddress)
                && !string.IsNullOrEmpty(order.Payer)
                && order.ShippingWarehouseId.HasValue
                && order.DeliveryWarehouseId.HasValue
                && order.ShippingDate.HasValue
                && order.DeliveryDate.HasValue;

            if (order.Status == OrderState.Draft && hasRequiredFields)
            {
                order.Status = OrderState.Created;
                _historyService.Save(order.Id, "orderSetCreated", order.OrderNumber);
            }
        }

        public bool IsTriggered(EntityChanges<Order> changes)
        {
            var watchProperties = new[]
            {
                nameof(Order.SoldTo),
                nameof(Order.ShippingAddress),
                nameof(Order.DeliveryAddress),
                nameof(Order.Payer),
                nameof(Order.ShippingWarehouseId),
                nameof(Order.DeliveryWarehouseId),
                nameof(Order.ShippingDate),
                nameof(Order.DeliveryDate)
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

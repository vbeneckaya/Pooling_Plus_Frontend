using System.Linq;
using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Shared;

namespace Application.BusinessModels.Orders.Triggers
{
    public class OnChangeDeliveryDate : UpdateIntegratedBase, ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;
        
        public OnChangeDeliveryDate(ICommonDataService dataService) : 
            base(dataService)
        {
            _dataService = dataService;
        }

        public void Execute(Order entity)
        {
            if (entity.Status == OrderState.InShipping) 
                UpdateOrderFromIntegrations(entity);
        }

        public bool IsTriggered(EntityChanges<Order> changes)
        {
            var watchProperties = new[]
            {
                nameof(Order.DeliveryDate),
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}
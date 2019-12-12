using Application.BusinessModels.Shared.Handlers;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Warehouses.Handlers
{
    public class AvisaleTimeHandler : IFieldHandler<Warehouse, TimeSpan?>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public AvisaleTimeHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public void AfterChange(Warehouse entity, TimeSpan? oldValue, TimeSpan? newValue)
        {
            var validStatuses = new[] { OrderState.Draft, OrderState.Created, OrderState.Confirmed, OrderState.InShipping };
            var orders = _dataService.GetDbSet<Order>()
                                     .Where(x => x.DeliveryWarehouseId == entity.Id && validStatuses.Contains(x.Status) && !x.ManualClientAvisationTime)
                                     .ToList();

            foreach (var order in orders)
            {
                _historyService.SaveImpersonated(null, order.Id, "fieldChanged", 
                                                 nameof(order.ClientAvisationTime).ToLowerFirstLetter(),
                                                 order.ClientAvisationTime, newValue);
                order.ClientAvisationTime = newValue;
            }
        }

        public string ValidateChange(Warehouse entity, TimeSpan? oldValue, TimeSpan? newValue)
        {
            return null;
        }
    }
}

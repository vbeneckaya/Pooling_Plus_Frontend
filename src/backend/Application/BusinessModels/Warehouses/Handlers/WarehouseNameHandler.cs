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
    public class WarehouseNameHandler : IFieldHandler<Warehouse, string>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public WarehouseNameHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public void AfterChange(Warehouse entity, string oldValue, string newValue)
        {
            var validStatuses = new[] { OrderState.Draft, OrderState.Created, OrderState.InShipping };
            var orders = _dataService.GetDbSet<Order>()
                                     .Where(x => x.SoldTo == entity.SoldToNumber
                                                && x.ClientName != newValue
                                                && validStatuses.Contains(x.Status)
                                                && (x.ShippingId == null || x.OrderShippingStatus == ShippingState.ShippingCreated))
                                     .ToList();

            foreach (var order in orders)
            {
                _historyService.SaveImpersonated(null, order.Id, "fieldChanged", 
                                                 nameof(order.ClientName).ToLowerFirstLetter(),
                                                 order.ClientName, newValue);
                order.ClientName = newValue;
            }
        }

        public string ValidateChange(Warehouse entity, string oldValue, string newValue)
        {
            return null;
        }
    }
}

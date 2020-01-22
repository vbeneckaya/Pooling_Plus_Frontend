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
    public class RegionHandler : IFieldHandler<Warehouse, string>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public RegionHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public void AfterChange(Warehouse entity, string oldValue, string newValue)
        {
            var validStatuses = new[] { OrderState.Created, OrderState.InShipping };
            var orders = _dataService.GetDbSet<Order>()
                                     .Where(x => x.DeliveryWarehouseId == entity.Id
                                                && x.DeliveryRegion != newValue
                                                && validStatuses.Contains(x.Status)
                                                && (x.ShippingId == null || x.OrderShippingStatus == ShippingState.ShippingCreated))
                                     .ToList();

            foreach (var order in orders)
            {
                _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                 nameof(order.DeliveryRegion).ToLowerFirstLetter(),
                                                 order.DeliveryRegion, newValue);
                order.DeliveryRegion = newValue;
            }
        }

        public string ValidateChange(Warehouse entity, string oldValue, string newValue)
        {
            return null;
        }
    }
}

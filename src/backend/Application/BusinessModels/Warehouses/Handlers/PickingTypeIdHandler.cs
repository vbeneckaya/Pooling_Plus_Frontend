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
    public class PickingTypeIdHandler : IFieldHandler<Warehouse, Guid?>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public PickingTypeIdHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public void AfterChange(Warehouse entity, Guid? oldValue, Guid? newValue)
        {
            var validStatuses = new[] { OrderState.Draft, OrderState.Created, OrderState.Confirmed, OrderState.InShipping };
            var orders = _dataService.GetDbSet<Order>()
                                     .Where(x => x.SoldTo == entity.SoldToNumber
                                                && !x.ManualPickingTypeId
                                                && x.PickingTypeId != newValue
                                                && x.Source != null && x.Source.Length > 0
                                                && validStatuses.Contains(x.Status)
                                                && (x.ShippingId == null || x.OrderShippingStatus == ShippingState.ShippingCreated))
                                     .ToList();

            string valueName = newValue == null ? null : _dataService.GetById<PickingType>(newValue.Value)?.Name;

            foreach (var order in orders)
            {
                _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                 nameof(order.PickingTypeId).ToLowerFirstLetter(),
                                                 order.PickingTypeId, valueName);
                order.PickingTypeId = newValue;
            }
        }

        public string ValidateChange(Warehouse entity, Guid? oldValue, Guid? newValue)
        {
            return null;
        }
    }
}

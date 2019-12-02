using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL.Queries;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class SoldToHandler : IFieldHandler<Order, string>
    {

        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, string oldValue, string newValue)
        {
            var setter = new FieldSetter<Order>(order);

            if (!string.IsNullOrEmpty(order.SoldTo))
            {

                var soldToWarehouse = _dataService.GetDbSet<Warehouse>().FirstOrDefault(x => x.SoldToNumber == order.SoldTo);
                if (soldToWarehouse != null)
                {
                    setter.UpdateField(o => o.ClientName, soldToWarehouse.WarehouseName);
                    setter.UpdateField(o => o.PickingFeatures, soldToWarehouse.PickingFeatures);

                    if (soldToWarehouse.PickingTypeId.HasValue)
                        setter.UpdateField(o => o.PickingTypeId, soldToWarehouse.PickingTypeId, nameLoader: GetPickingTypeNameById);

                    setter.UpdateField(o => o.TransitDays, soldToWarehouse.LeadtimeDays);

                    setter.UpdateField(o => o.DeliveryWarehouseId, soldToWarehouse.Id, ignoreChanges: true);
                    setter.UpdateField(o => o.DeliveryAddress, soldToWarehouse.Address);
                    setter.UpdateField(o => o.DeliveryCity, soldToWarehouse.City);
                    setter.UpdateField(o => o.DeliveryRegion, soldToWarehouse.Region);
                    setter.UpdateField(o => o.DeliveryType, soldToWarehouse.DeliveryType);
                }
                else
                {
                    order.DeliveryWarehouseId = null;
                }
            }

            setter.UpdateField(o => o.ShippingDate, order.DeliveryDate?.AddDays(0 - order.TransitDays ?? 0));
            setter.UpdateField(o => o.OrderChangeDate, DateTime.UtcNow, ignoreChanges: true);
        }

        public string ValidateChange(Order order, string oldValue, string newValue)
        {
            return null;
        }

        private string GetPickingTypeNameById(Guid? id)
        {
            return id == null ? null : _dataService.GetDbSet<PickingType>().GetById(id.Value)?.Name;
        }

        public SoldToHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }
    }
}

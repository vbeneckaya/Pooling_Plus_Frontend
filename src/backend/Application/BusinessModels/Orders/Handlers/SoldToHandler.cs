using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class SoldToHandler : IFieldHandler<Order, string>
    {
        public void AfterChange(Order order, string oldValue, string newValue)
        {
            if (!string.IsNullOrEmpty(order.SoldTo))
            {
                var soldToWarehouse = _db.Warehouses.FirstOrDefault(x => x.SoldToNumber == order.SoldTo);
                if (soldToWarehouse != null)
                {
                    var setter = new FieldSetter<Order>(order, _historyService);

                    setter.UpdateField(o => o.ClientName, soldToWarehouse.WarehouseName);

                    if (soldToWarehouse.UsePickingType)
                        setter.UpdateField(o => o.PickingTypeId, soldToWarehouse.PickingTypeId, nameLoader: GetPickingTypeNameById);

                    setter.UpdateField(o => o.TransitDays, soldToWarehouse.LeadtimeDays);
                    setter.UpdateField(o => o.ShippingDate, order.DeliveryDate?.AddDays(0 - order.TransitDays ?? 0));

                    setter.UpdateField(o => o.DeliveryWarehouseId, soldToWarehouse.Id, ignoreChanges: true);
                    setter.UpdateField(o => o.DeliveryAddress, soldToWarehouse.Address);
                    setter.UpdateField(o => o.DeliveryCity, soldToWarehouse.City);
                    setter.UpdateField(o => o.DeliveryRegion, soldToWarehouse.Region);

                    setter.SaveHistoryLog();
                }
            }
        }

        public string ValidateChange(Order order, string oldValue, string newValue)
        {
            return null;
        }

        private string GetPickingTypeNameById(Guid? id)
        {
            return id == null ? null : _db.PickingTypes.GetById(id.Value)?.Name;
        }

        public SoldToHandler(AppDbContext db, IHistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        private readonly AppDbContext _db;
        private readonly IHistoryService _historyService;
    }
}

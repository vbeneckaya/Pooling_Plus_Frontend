using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.History;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class DeliveryStatusHandler : IFieldHandler<Order, VehicleState>
    {
        public void AfterChange(Order order, VehicleState oldValue, VehicleState newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var ordersToUpdate = _db.Orders.Where(o => o.ShippingId == order.ShippingId
                                                        && o.Id != order.Id
                                                        && o.DeliveryWarehouseId == order.DeliveryWarehouseId)
                                               .ToList();

                foreach (Order updOrder in ordersToUpdate)
                {
                    var setter = new FieldSetter<Order>(updOrder, _historyService);
                    setter.UpdateField(o => o.DeliveryStatus, newValue);
                    setter.SaveHistoryLog();
                }
            }
        }

        public string ValidateChange(Order order, VehicleState oldValue, VehicleState newValue)
        {
            return null;
        }

        public DeliveryStatusHandler(AppDbContext db, IHistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        private readonly AppDbContext _db;
        private readonly IHistoryService _historyService;
    }
}

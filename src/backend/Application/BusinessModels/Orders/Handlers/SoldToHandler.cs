using Application.BusinessModels.Shared.Handlers;
using DAL;
using Domain.Persistables;
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
                    order.ClientName = soldToWarehouse.WarehouseName;

                    if (soldToWarehouse.UsePickingType == "Да")
                        order.PickingType = soldToWarehouse.PickingType;

                    if (!string.IsNullOrEmpty(soldToWarehouse.LeadtimeDays))
                    {
                        int leadTimeDays = int.Parse(soldToWarehouse.LeadtimeDays);
                        order.TransitDays = leadTimeDays;
                    }

                    order.ShippingDate = order.DeliveryDate?.AddDays(0 - order.TransitDays ?? 0);

                    order.DeliveryAddress = soldToWarehouse.Address;
                    order.DeliveryCity = soldToWarehouse.City;
                    order.DeliveryRegion = soldToWarehouse.Region;
                }
            }
        }

        public string ValidateChange(Order order, string oldValue, string newValue)
        {
            return null;
        }

        public SoldToHandler(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}

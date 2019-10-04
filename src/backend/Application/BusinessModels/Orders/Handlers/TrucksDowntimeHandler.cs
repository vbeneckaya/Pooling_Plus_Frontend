using Application.BusinessModels.Shared.Handlers;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class TrucksDowntimeHandler : IFieldHandler<Order, decimal?>
    {
        public void AfterChange(Order order, decimal? oldValue, decimal? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var shipping = _db.Shippings.GetById(order.ShippingId.Value);
                if (shipping != null && !shipping.ManualTrucksDowntime)
                {
                    var downtimes = _db.Orders.Where(o => o.ShippingId == order.ShippingId && o.Id != order.Id)
                                             .Select(o => o.TrucksDowntime)
                                             .ToList();
                    downtimes.Add(newValue);

                    var shippingDowntime = downtimes.Any(x => x.HasValue) ? downtimes.Sum(x => x ?? 0) : (decimal?)null;
                    shipping.TrucksDowntime = shippingDowntime;
                }
            }
        }

        public string ValidateChange(Order order, decimal? oldValue, decimal? newValue)
        {
            return null;
        }

        public TrucksDowntimeHandler(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}

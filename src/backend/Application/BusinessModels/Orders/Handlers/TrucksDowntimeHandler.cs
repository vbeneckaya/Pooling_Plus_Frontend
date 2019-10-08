using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using Domain.Services.History;
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

                    var setter = new FieldSetter<Shipping>(shipping, _historyService);
                    setter.UpdateField(s => s.TrucksDowntime, shippingDowntime);
                    setter.SaveHistoryLog();
                }
            }
        }

        public string ValidateChange(Order order, decimal? oldValue, decimal? newValue)
        {
            return null;
        }

        public TrucksDowntimeHandler(AppDbContext db, IHistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        private readonly AppDbContext _db;
        private readonly IHistoryService _historyService;
    }
}

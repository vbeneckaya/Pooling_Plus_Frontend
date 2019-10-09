using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using Domain.Services.History;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class PalletsCountHandler : IFieldHandler<Order, int?>
    {
        public void AfterChange(Order order, int? oldValue, int? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var shipping = _db.Shippings.GetById(order.ShippingId.Value);
                if (shipping != null && !shipping.ManualPalletsCount)
                {
                    var counts = _db.Orders.Where(o => o.ShippingId == order.ShippingId && o.Id != order.Id)
                                           .Select(o => o.PalletsCount)
                                           .ToList();
                    counts.Add(newValue);

                    var shippingPalletsCount = counts.Any(x => x.HasValue) ? counts.Sum(x => x ?? 0) : (int?)null;

                    var setter = new FieldSetter<Shipping>(shipping, _historyService);
                    setter.UpdateField(s => s.PalletsCount, shippingPalletsCount);
                    setter.SaveHistoryLog();
                }
            }
        }

        public string ValidateChange(Order order, int? oldValue, int? newValue)
        {
            return null;
        }

        public PalletsCountHandler(AppDbContext db, IHistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        private readonly AppDbContext _db;
        private readonly IHistoryService _historyService;
    }
}

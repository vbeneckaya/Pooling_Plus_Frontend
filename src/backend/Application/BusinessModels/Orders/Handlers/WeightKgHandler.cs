using Application.BusinessModels.Shared.Handlers;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class WeightKgHandler : IFieldHandler<Order, decimal?>
    {
        public void AfterChange(Order order, decimal? oldValue, decimal? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var shipping = _db.Shippings.GetById(order.ShippingId.Value);
                if (shipping != null && !shipping.ManualWeightKg)
                {
                    var weights = _db.Orders.Where(o => o.ShippingId == order.ShippingId && o.Id != order.Id)
                                            .Select(o => o.WeightKg)
                                            .ToList();
                    weights.Add(newValue);

                    var shippingWeight = weights.Any(x => x.HasValue) ? weights.Sum(x => x ?? 0) : (decimal?)null;
                    shipping.WeightKg = shippingWeight;
                }
            }
        }

        public string ValidateChange(Order order, decimal? oldValue, decimal? newValue)
        {
            return null;
        }

        public WeightKgHandler(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}

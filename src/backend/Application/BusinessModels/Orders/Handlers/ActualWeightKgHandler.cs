using Application.BusinessModels.Shared.Handlers;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ActualWeightKgHandler : IFieldHandler<Order, decimal?>
    {
        public void AfterChange(Order order, decimal? oldValue, decimal? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var shipping = _db.Shippings.GetById(order.ShippingId.Value);
                if (shipping != null && !shipping.ManualActualWeightKg)
                {
                    var actualWeights = _db.Orders.Where(o => o.ShippingId == order.ShippingId).Select(o => o.ActualWeightKg).ToList();
                    var shippingActualWeight = actualWeights.Any(x => x.HasValue) ? actualWeights.Sum(x => x ?? 0) : (decimal?)null;
                    shipping.ActualWeightKg = shippingActualWeight;
                }
            }
        }

        public string ValidateChange(Order order, decimal? oldValue, decimal? newValue)
        {
            return null;
        }

        public ActualWeightKgHandler(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}

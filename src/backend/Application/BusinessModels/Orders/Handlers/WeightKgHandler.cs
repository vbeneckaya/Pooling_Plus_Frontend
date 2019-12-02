using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class WeightKgHandler : IFieldHandler<Order, decimal?>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, decimal? oldValue, decimal? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var shipping = _dataService.GetById<Shipping>(order.ShippingId.Value);
                if (shipping != null && !shipping.ManualWeightKg)
                {
                    var weights = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == order.ShippingId && o.Id != order.Id)
                                            .Select(o => o.WeightKg)
                                            .ToList();
                    weights.Add(newValue);

                    var shippingWeight = weights.Any(x => x.HasValue) ? weights.Sum(x => x ?? 0) : (decimal?)null;

                    var setter = new FieldSetter<Shipping>(shipping);
                    setter.UpdateField(s => s.WeightKg, shippingWeight);
                }
            }

            order.OrderChangeDate = DateTime.UtcNow;
        }

        public string ValidateChange(Order order, decimal? oldValue, decimal? newValue)
        {
            return null;
        }

        public WeightKgHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }
    }
}

using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL;
using DAL.Queries;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ConfirmedWeightKgHandler : IFieldHandler<Order, decimal?>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, decimal? oldValue, decimal? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var shipping = _dataService.GetById<Shipping>(order.ShippingId.Value);
                if (shipping != null && !shipping.ManualConfirmedWeightKg)
                {
                    var confirmedWeights = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == order.ShippingId && o.Id != order.Id)
                                                  .Select(o => o.ConfirmedWeightKg)
                                                  .ToList();
                    confirmedWeights.Add(newValue);

                    var shippingActualWeight = confirmedWeights.Any(x => x.HasValue) ? confirmedWeights.Sum(x => x ?? 0) : (decimal?)null;
                    shipping.ActualWeightKg = shippingActualWeight;
                }
            }
        }

        public string ValidateChange(Order order, decimal? oldValue, decimal? newValue)
        {
            return null;
        }

        public ConfirmedWeightKgHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }
    }
}

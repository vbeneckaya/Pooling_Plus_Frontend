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
    public class TrucksDowntimeHandler : IFieldHandler<Order, decimal?>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, decimal? oldValue, decimal? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var shipping = _dataService.GetById<Shipping>(order.ShippingId.Value);
                if (shipping != null && !shipping.ManualTrucksDowntime)
                {
                    var ordersToUpdate = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == order.ShippingId
                                                            && o.Id != order.Id
                                                            && o.DeliveryWarehouseId == order.DeliveryWarehouseId)
                                                   .ToList();

                    foreach (Order updOrder in ordersToUpdate)
                    {
                        updOrder.TrucksDowntime = newValue;
                    }

                    var downtimes = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == order.ShippingId && o.Id != order.Id)
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

        public TrucksDowntimeHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }
    }
}

using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ConfirmedPalletsCountHandler : IFieldHandler<Order, int?>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, int? oldValue, int? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var shipping = _dataService.GetById<Shipping>(order.ShippingId.Value);
                if (shipping != null && !shipping.ManualConfirmedPalletsCount)
                {
                    var counts = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == order.ShippingId && o.Id != order.Id)
                                           .Select(o => o.ConfirmedPalletsCount)
                                           .ToList();
                    counts.Add(newValue);

                    var shippingConfirmedPalletsCount = counts.Any(x => x.HasValue) ? counts.Sum(x => x ?? 0) : (int?)null;

                    var setter = new FieldSetter<Shipping>(shipping, _historyService);
                    setter.UpdateField(s => s.ConfirmedPalletsCount, shippingConfirmedPalletsCount);
                    setter.SaveHistoryLog();
                }
            }

            if (newValue != order.PalletsCount)
            {
                order.OrderChangeDate = DateTime.Now;
            }
        }

        public string ValidateChange(Order order, int? oldValue, int? newValue)
        {
            return null;
        }

        public ConfirmedPalletsCountHandler(ICommonDataService dataService, IHistoryService historyService)
        {
             _dataService = dataService;
            _historyService = historyService;
        }
    }
}

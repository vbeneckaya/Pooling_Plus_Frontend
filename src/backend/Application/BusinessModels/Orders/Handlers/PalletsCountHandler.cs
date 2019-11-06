using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class PalletsCountHandler : IFieldHandler<Order, int?>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        private readonly bool _isManual;

        public PalletsCountHandler(ICommonDataService dataService, IHistoryService historyService, bool isManual)
        {
            this._dataService = dataService;
            _historyService = historyService;
            _isManual = isManual;
        }

        public void AfterChange(Order order, int? oldValue, int? newValue)
        {
            if (_isManual)
            {
                order.ManualPalletsCount = true;
            }

            if (order.ShippingId.HasValue)
            {
                var shipping = _dataService.GetById<Shipping>(order.ShippingId.Value);
                if (shipping != null && !shipping.ManualPalletsCount)
                {
                    var counts = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == order.ShippingId && o.Id != order.Id)
                                           .Select(o => o.PalletsCount)
                                           .ToList();
                    counts.Add(newValue);

                    var shippingPalletsCount = counts.Any(x => x.HasValue) ? counts.Sum(x => x ?? 0) : (int?)null;

                    var setter = new FieldSetter<Shipping>(shipping, _historyService);
                    setter.UpdateField(s => s.PalletsCount, shippingPalletsCount);
                    setter.SaveHistoryLog();
                }
            }

            var orderSetter = new FieldSetter<Order>(order, _historyService);
            orderSetter.UpdateField(s => s.OrderChangeDate, DateTime.Now);
            orderSetter.SaveHistoryLog();
        }

        public string ValidateChange(Order order, int? oldValue, int? newValue)
        {
            return null;
        }
    }
}

using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Shippings.Handlers
{
    public class LoadingArrivalTimeHandler : IFieldHandler<Shipping, DateTime?>
    {
        public void AfterChange(Shipping shipping, DateTime? oldValue, DateTime? newValue)
        {
                var ordersToUpdate = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == shipping.Id).ToList();

                foreach (Order updOrder in ordersToUpdate)
                {
                    var setter = new FieldSetter<Order>(updOrder, _historyService);
                    setter.UpdateField(o => o.LoadingArrivalTime, newValue);
                    setter.SaveHistoryLog();
                }
        }

        public string ValidateChange(Shipping shipping, DateTime? oldValue, DateTime? newValue)
        {
            return null;
        }

        public LoadingArrivalTimeHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
    }
}

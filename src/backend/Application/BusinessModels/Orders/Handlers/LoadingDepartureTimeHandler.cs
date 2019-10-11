using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class LoadingDepartureTimeHandler : IFieldHandler<Order, DateTime?>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var ordersToUpdate = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == order.ShippingId
                                                        && o.Id != order.Id
                                                        && o.ShippingWarehouseId == order.ShippingWarehouseId)
                                               .ToList();

                foreach (Order updOrder in ordersToUpdate)
                {
                    var setter = new FieldSetter<Order>(updOrder, _historyService);
                    setter.UpdateField(o => o.LoadingDepartureTime, newValue);
                    setter.SaveHistoryLog();
                }

                var shipping = _dataService.GetById<Shipping>(order.ShippingId.Value);
                if (shipping != null)
                {
                    var setter = new FieldSetter<Shipping>(shipping, _historyService);
                    setter.UpdateField(s => s.LoadingDepartureTime, newValue);
                    setter.SaveHistoryLog();
                }
            }
        }

        public string ValidateChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.LoadingArrivalTime.HasValue && newValue.HasValue && order.LoadingArrivalTime > newValue)
            {
                return $"Время убытия со склада БДФ не может быть раньше Времени прибытия на загрузку (склад БДФ)";
            }
            else if (order.UnloadingArrivalTime.HasValue && newValue.HasValue && order.UnloadingArrivalTime < newValue)
            {
                return $"Время убытия со склада БДФ не может быть позже Времени прибытия к грузополучателю";
            }
            else
            {
                return null;
            }
        }

        public LoadingDepartureTimeHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }
    }
}

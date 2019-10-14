using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class UnloadingArrivalTimeHandler : IFieldHandler<Order, DateTime?>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                var ordersToUpdate = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == order.ShippingId
                                                        && o.Id != order.Id
                                                        && o.DeliveryWarehouseId == order.DeliveryWarehouseId)
                                               .ToList();

                foreach (Order updOrder in ordersToUpdate)
                {
                    var setter = new FieldSetter<Order>(updOrder, _historyService);
                    setter.UpdateField(o => o.UnloadingArrivalTime, newValue);
                    setter.SaveHistoryLog();
                }
            }
        }

        public string ValidateChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.UnloadingDepartureTime.HasValue && newValue.HasValue && order.UnloadingDepartureTime < newValue)
            {
                return $"Время убытия со грузополучателя не может быть раньше Времени прибытия к грузополучателю";
            }
            else if (order.LoadingDepartureTime.HasValue && newValue.HasValue && order.LoadingDepartureTime > newValue)
            {
                return $"Время убытия со склада БДФ не может быть позже Времени прибытия к грузополучателю";
            }
            else
            {
                return null;
            }
        }

        public UnloadingArrivalTimeHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }
    }
}

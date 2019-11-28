using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL;
using DAL.Queries;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class LoadingArrivalTimeHandler : IFieldHandler<Order, DateTime?>
    {
        public void AfterChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.ShippingId.HasValue)
            {
                //var ordersToUpdate = _dataService.GetDbSet<Order>().Where(o => o.ShippingId == order.ShippingId
                //                                        && o.Id != order.Id
                //                                        && o.ShippingWarehouseId == order.ShippingWarehouseId)
                //                               .ToList();

                //foreach (Order updOrder in ordersToUpdate)
                //{
                //    var setter = new FieldSetter<Order>(updOrder, _historyService);
                //    setter.UpdateField(o => o.LoadingArrivalTime, newValue);
                //    setter.SaveHistoryLog();
                //}

                var shipping = _dataService.GetById<Shipping>(order.ShippingId.Value);
                if (shipping != null)
                {
                    var orders = _dataService.GetDbSet<Order>()
                                             .Where(o => o.ShippingId == order.ShippingId && o.Id != order.Id)
                                             .ToList();
                    orders.Add(order);

                    var loadingArrivalTime = orders.Select(i => i.LoadingArrivalTime).Where(i => i != null).Min();

                    var setter = new FieldSetter<Shipping>(shipping, _historyService);
                    setter.UpdateField(s => s.LoadingArrivalTime, loadingArrivalTime);
                    setter.SaveHistoryLog();
                }
            }
        }

        public string ValidateChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.LoadingDepartureTime.HasValue && newValue.HasValue && order.LoadingDepartureTime < newValue)
            {
                return $"Время убытия со склада БДФ не может быть раньше Времени прибытия на загрузку (склад БДФ)";
            }
            else
            {
                return null;
            }
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

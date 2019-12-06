using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class DeliveryDateHandler : IFieldHandler<Order, DateTime?>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly bool _isInjection;

        public DeliveryDateHandler(ICommonDataService dataService, IHistoryService historyService, bool isInjection)
        {
            _dataService = dataService;
            _historyService = historyService;
            _isInjection = isInjection;
        }

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
                    updOrder.DeliveryDate = newValue;
                }
            }

            if (_isInjection)
            {
                order.ShippingDate = newValue?.AddDays(0 - order.TransitDays ?? 0);
            }
            else
            {
                order.ManualDeliveryDate = true;
            }

            order.OrderChangeDate = DateTime.UtcNow;
        }

        public string ValidateChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.ShippingDate.HasValue && newValue.HasValue && order.ShippingDate > newValue)
            {
                return $"Дата отгрузки не может быть больше даты доставки.";
            }
            else
            {
                return null;
            }
        }
    }
}

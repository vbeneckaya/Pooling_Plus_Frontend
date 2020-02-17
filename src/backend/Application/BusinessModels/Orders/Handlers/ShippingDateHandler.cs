using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ShippingDateHandler : IFieldHandler<Order, DateTime?>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly bool _isManual;

        public ShippingDateHandler(ICommonDataService dataService, IHistoryService historyService, bool isManual)
        {
            _dataService = dataService;
            _historyService = historyService;
            _isManual = isManual;
        }

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
                    updOrder.ShippingDate = newValue;
                }
            }

            order.OrderChangeDate = DateTime.UtcNow;

            if (_isManual)
            {
                order.ManualShippingDate = true;
            }
        }

        public string ValidateChange(Order order, DateTime? oldValue, DateTime? newValue)
        {
            if (order.DeliveryDate.HasValue && newValue.HasValue && order.DeliveryDate < newValue)
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

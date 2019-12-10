using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ShippingWarehouseHandler : IFieldHandler<Order, Guid?>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, Guid? oldValue, Guid? newValue)
        {
            if (order.ShippingWarehouseId.HasValue)
            {
                var shippingWarehouse = _dataService.GetDbSet<ShippingWarehouse>().Find(order.ShippingWarehouseId.Value);
                if (shippingWarehouse != null)
                {
                    order.ShippingAddress = shippingWarehouse.Address;
                    order.ShippingCity = shippingWarehouse.City;
                }
            }
        }

        public string ValidateChange(Order order, Guid? oldValue, Guid? newValue)
        {
            return null;
        }

        public ShippingWarehouseHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

    }
}

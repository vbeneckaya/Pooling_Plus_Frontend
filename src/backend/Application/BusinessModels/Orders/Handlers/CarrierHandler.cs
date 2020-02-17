using Application.BusinessModels.Shared.Handlers;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class CarrierHandler : IFieldHandler<Order, Guid?>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, Guid? oldValue, Guid? newValue)
        {
            if (order.ShippingId != null)
            {
                string oldName = oldValue == null ? null : _dataService.GetById<TransportCompany>(oldValue.Value)?.Title;
                string newName = newValue == null ? null : _dataService.GetById<TransportCompany>(newValue.Value)?.Title;

                var shipping = _dataService.GetById<Shipping>(order.ShippingId.Value);
                if (shipping != null)
                {
                    shipping.CarrierId = newValue;
                    _historyService.Save(shipping.Id, "fieldChanged", 
                                         nameof(shipping.CarrierId).ToLowerFirstLetter(),
                                         oldName, newName);
                }

                var otherOrders = _dataService.GetDbSet<Order>().Where(x => x.ShippingId == order.ShippingId && x.Id != order.Id).ToList();
                foreach (var otherOrder in otherOrders)
                {
                    otherOrder.CarrierId = newValue;
                    _historyService.Save(otherOrder.Id, "fieldChanged",
                                         nameof(otherOrder.CarrierId).ToLowerFirstLetter(),
                                         oldName, newName);
                }
            }
        }

        public string ValidateChange(Order order, Guid? oldValue, Guid? newValue)
        {
            return null;
        }

        public CarrierHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

    }
}

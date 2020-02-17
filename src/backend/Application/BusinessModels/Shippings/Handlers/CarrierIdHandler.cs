using Application.BusinessModels.Shared.Handlers;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Shippings.Handlers
{
    public class CarrierIdHandler : IFieldHandler<Shipping, Guid?>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public CarrierIdHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public void AfterChange(Shipping shipping, Guid? oldValue, Guid? newValue)
        {
            var oldName = oldValue == null ? null : _dataService.GetById<TransportCompany>(oldValue.Value)?.Title;
            var newName = newValue == null ? null : _dataService.GetById<TransportCompany>(newValue.Value)?.Title;

            var orders = _dataService.GetDbSet<Order>()
                                     .Where(x => x.ShippingId == shipping.Id)
                                     .ToList();

            foreach (var order in orders)
            {
                _historyService.Save(order.Id, "fieldChanged", 
                                     nameof(order.CarrierId).ToLowerFirstLetter(),
                                     oldName, newName);
                order.CarrierId = newValue;
            }
        }

        public string ValidateChange(Shipping shipping, Guid? oldValue, Guid? newValue)
        {
            return null;
        }
    }
}

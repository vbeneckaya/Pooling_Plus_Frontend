using System;
using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;
using Application.Services.Shippings;
using DAL.Queries;
using Domain.Extensions;
using Domain.Services.History;
using Domain.Services.Shippings;

namespace Application.BusinessModels.Shippings.Triggers
{
    public class OnChangeTransportCompany : ITrigger<Shipping>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly IDeliveryCostCalcService _calcService;        

        public OnChangeTransportCompany(ICommonDataService dataService, IHistoryService historyService, IDeliveryCostCalcService calcService)
        {
            _dataService = dataService;
            _historyService = historyService;
            _calcService = calcService;
        }

        public void Execute(Shipping entity)
        {
            //entity.TotalDeliveryCost = new Random().Next(1000,15000);
            
            var orders = _dataService.GetDbSet<Order>()
                .Where(x => x.ShippingId == entity.Id);

            var transportCompanies = _dataService.GetDbSet<TransportCompany>();
            
            foreach (var orderInShipping in orders)
            {
                if (orderInShipping.CarrierId != entity.CarrierId)
                {
                    TransportCompany oldCarrier = null;
                    TransportCompany newCarrier = null;

                    if (orderInShipping.CarrierId.HasValue)
                        oldCarrier = transportCompanies.GetById(orderInShipping.VehicleTypeId.Value);

                    if (entity.VehicleTypeId.HasValue)
                        newCarrier = transportCompanies.GetById(entity.VehicleTypeId.Value);

                    orderInShipping.CarrierId = entity.CarrierId;

                    _historyService.Save(orderInShipping.Id, "fieldChangedBy",
                        nameof(orderInShipping.VehicleTypeId).ToLowerFirstLetter(),
                        oldCarrier, newCarrier, "onChangeInShipping");
                }
                if(!entity.ManualTarifficationType)
                    _calcService.UpdateDeliveryCost(entity);

            }
        }

        public bool IsTriggered(EntityChanges<Shipping> changes)
        {
            var watchProperties = new[]
            {
                nameof(Shipping.CarrierId),
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

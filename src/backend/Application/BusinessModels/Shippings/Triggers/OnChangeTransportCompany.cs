using System;
using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Linq;
using Application.Services.Shippings;
using DAL.Queries;
using Domain.Enums;
using Domain.Extensions;
using Domain.Services.History;
using Domain.Services.Shippings;
using Integrations.Pooling;

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

        public void Execute(Shipping shipping)
        {
            if (shipping.CarrierId != null && shipping.VehicleTypeId != null)
            {
                var creator = _dataService.GetById<User>(shipping.UserCreatorId);
                
                using (var poolingIntegration = new PoolingIntegration(creator, _dataService))
                {
                    var poolingInfo = poolingIntegration.GetInfoFor(shipping);
                    
                    if (poolingInfo.IsAvailable)
                        shipping.PoolingState = ShippingPoolingState.PoolingAvailable;
                    else
                        shipping.PoolingState = null;

                    shipping.PoolingInfo = poolingInfo.MessageField;
                }
            }
        }

        public bool IsTriggered(EntityChanges<Shipping> changes)
        {
            var watchProperties = new[]
            {
                nameof(Shipping.CarrierId),
                nameof(Shipping.VehicleTypeId),
                nameof(Shipping.BodyTypeId),
                nameof(Shipping.TarifficationType)
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

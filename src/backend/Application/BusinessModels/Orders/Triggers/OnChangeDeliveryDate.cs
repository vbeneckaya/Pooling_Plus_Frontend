using System.Linq;
using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Services.Shippings;
using Domain.Shared;
using Integrations.Pooling;

namespace Application.BusinessModels.Orders.Triggers
{
    public class OnChangeDeliveryDate : ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;

        public OnChangeDeliveryDate(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        public void Execute(Order entity)
        {
            if (entity.Status == OrderState.InShipping)
            {
                if (entity.ShippingId.HasValue)
                {
                    var entityShippingId = entity.ShippingId.Value;

                    var shipping = _dataService.GetById<Shipping>(entityShippingId);

                    if (shipping.Status == ShippingState.ShippingCreated)
                    {
                        var creator = _dataService.GetById<User>(shipping.UserCreatorId);
                        if (creator.IsPoolingIntegrated())
                        {
                            using (var poolingIntegration = new PoolingIntegration(creator, _dataService))
                            {
                                var poolingInfo = poolingIntegration.GetInfoFor(shipping);
                                shipping.PoolingState = poolingInfo.IsAvailable
                                    ? ShippingPoolingState.PoolingAvailable : (ShippingPoolingState?)null;
                                shipping.PoolingInfo = poolingInfo.MessageField;
                            }
                        }

                    }
                }
            }
        }

        public bool IsTriggered(EntityChanges<Order> changes)
        {
            var watchProperties = new[]
            {
                nameof(Order.DeliveryDate),
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}
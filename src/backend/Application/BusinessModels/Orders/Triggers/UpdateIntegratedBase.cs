using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Integrations.Pooling;

namespace Application.BusinessModels.Orders.Triggers
{
    public abstract class UpdateIntegratedBase
    {
        private readonly ICommonDataService _dataService;

        protected UpdateIntegratedBase(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        protected void UpdateShippingFromIntegrations(Shipping shipping)
        {
            if (shipping.Status == ShippingState.ShippingCreated)
            {
                var creator = _dataService.GetByIdOrNull<User>(shipping.UserCreatorId);
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

        protected void UpdateOrderFromIntegrations(Order order)
        {
            if (order.ShippingId.HasValue)
            {
                UpdateShippingFromIntegrations(
                    _dataService.GetByIdOrNull<Shipping>(order.ShippingId));
            }
        }
    }
}
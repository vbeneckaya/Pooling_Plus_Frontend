using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.ShippingWarehouses;
using Integrations.Pooling;

namespace Application.BusinessModels.Shared.Triggers
{
    public abstract class UpdateIntegratedBase
    {
        private readonly ICommonDataService _dataService;
        private readonly IShippingWarehousesService _shippingWarehousesService;

        protected UpdateIntegratedBase(ICommonDataService dataService, IShippingWarehousesService shippingWarehousesService = null)
        {
            _dataService = dataService;
           _shippingWarehousesService = shippingWarehousesService;
        }

        protected void UpdateShippingFromIntegrations(Shipping shipping)
        {
            if (shipping.Status == ShippingState.ShippingCreated && shipping.UserCreatorId.HasValue)
            {
                if (shipping.CarrierId == null)
                {
                    shipping.PoolingInfo = "Укажите Транспортную компанию";
                    return;
                }
                if (shipping.TarifficationType == null)
                {
                    shipping.PoolingInfo = "Укажите Способ тарификации";
                    return;
                }
                var creator = _dataService.GetByIdOrNull<User>(shipping.UserCreatorId);
                if (creator.IsPoolingIntegrated())
                {
                    using (var poolingIntegration = new PoolingIntegration(creator, _dataService, _shippingWarehousesService))
                    {
                        var poolingInfo = poolingIntegration.GetInfoFor(shipping);
                        shipping.PoolingState = poolingInfo.IsAvailable
                            ? ShippingPoolingState.PoolingAvailable : (ShippingPoolingState?)null;
                        shipping.PoolingInfo = poolingInfo.MessageField;
                    }
                }
                else
                {
                    shipping.PoolingInfo = PoolingIntegration.NeedLoginPasswordMessage;
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
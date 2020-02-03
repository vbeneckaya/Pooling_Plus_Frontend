using Application.BusinessModels.Shared.Triggers;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Shared;
using System.Linq;
using Domain.Services.Shippings;

namespace Application.BusinessModels.Orders.Triggers
{
    public class UpdateShippingRoute : ITrigger<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IShippingGetRouteService _getRouteService;

        public UpdateShippingRoute(ICommonDataService dataService, IShippingGetRouteService getRouteService)
        {
            _dataService = dataService;
            _getRouteService = getRouteService;
        }

        public void Execute(Order entity)
        {
            if (entity.ShippingId != null)
            {
                var shipping = _dataService.GetById<Shipping>(entity.ShippingId.Value);
                var orders = _dataService.GetDbSet<Order>().Where(_ => _.ShippingId == entity.ShippingId);
                _getRouteService.UpdateRoute(shipping, orders);
            }
        }

        public bool IsTriggered(EntityChanges<Order> changes)
        {
            var watchProperties = new[]
            {
                nameof(Order.DeliveryWarehouseId),
                nameof(Order.ShippingWarehouseId),
                nameof(Order.ShippingDate),
                nameof(Order.DeliveryDate)
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}

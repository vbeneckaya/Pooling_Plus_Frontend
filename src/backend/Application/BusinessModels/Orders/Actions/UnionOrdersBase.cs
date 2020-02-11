using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.History;
using System.Collections.Generic;
using Domain.Services.Shippings;

namespace Application.BusinessModels.Orders.Actions
{
    public abstract class UnionOrdersBase
    {
        protected readonly ICommonDataService _dataService;
        protected readonly IShippingCalculationService _shippingCalculationService;
        protected readonly IShippingGetRouteService _shippingGetRouteService;

        public UnionOrdersBase(ICommonDataService dataService, IShippingCalculationService shippingCalculationService, IShippingGetRouteService shippingGetRouteService)
        {
            _dataService = dataService;
            _shippingCalculationService = shippingCalculationService;
            _shippingGetRouteService = shippingGetRouteService;
        }

        protected void UnionOrderInShipping(IEnumerable<Order> allOrders, IEnumerable<Order> newOrders, Shipping shipping, IHistoryService historyService)
        {
            _shippingCalculationService.RecalculateShipping(shipping, allOrders);
            _shippingGetRouteService.UpdateRoute(shipping, allOrders);

            foreach (var order in allOrders)
            {
                order.ShippingNumber = shipping.ShippingNumber;
                order.OrderShippingStatus = shipping.Status;
            }

            foreach (var order in newOrders)
            {
                order.ShippingId = shipping.Id;
                order.Status = OrderState.InShipping;

                order.ShippingStatus = VehicleState.VehicleWaiting;
                order.DeliveryStatus = VehicleState.VehicleEmpty;
                order.CarrierId = shipping.CarrierId;

                historyService.Save(order.Id, "orderSetInShipping", order.OrderNumber, shipping.ShippingNumber);
                historyService.Save(shipping.Id, "shippingAddOrder", order.OrderNumber, shipping.ShippingNumber);
            }
            
        }
    }
}
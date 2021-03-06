using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.History;
using System.Collections.Generic;
using Application.BusinessModels.Shared.Triggers;
using Domain.Services.Shippings;

namespace Application.BusinessModels.Orders.Actions
{
    public abstract class UnionOrdersBase : UpdateIntegratedBase
    {
        protected readonly ICommonDataService _dataService;
        protected readonly IShippingCalculationService _shippingCalculationService;
        protected readonly IShippingGetRouteService _shippingGetRouteService;

        public UnionOrdersBase(ICommonDataService dataService, IShippingCalculationService shippingCalculationService, IShippingGetRouteService shippingGetRouteService)
        :base(dataService)
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
                if (order.Status == null || order.Status == OrderState.InShipping)
                {
                    order.Status = order.Status == OrderState.Created ? OrderState.InShipping : order.Status;
                    order.ShippingStatus = VehicleState.VehicleWaiting;
                    order.CarrierId = order.CarrierId ?? shipping.CarrierId;
                    historyService.Save(order.Id, "orderSetInShipping", order.OrderNumber, shipping.ShippingNumber);
                    historyService.Save(shipping.Id, "shippingAddOrder", order.OrderNumber, shipping.ShippingNumber);
                }
                order.ShippingNumber = shipping.ShippingNumber;
                order.OrderShippingStatus = shipping.Status;
            }

            foreach (var order in newOrders)
            {
                order.ShippingId = shipping.Id;
                order.Status = OrderState.InShipping;
                order.ShippingNumber = shipping.ShippingNumber;

                order.ShippingStatus = VehicleState.VehicleWaiting;
                order.DeliveryStatus = VehicleState.VehicleEmpty;
                order.CarrierId = shipping.CarrierId;

                historyService.Save(order.Id, "orderSetInShipping", order.OrderNumber, shipping.ShippingNumber);
                historyService.Save(shipping.Id, "shippingAddOrder", order.OrderNumber, shipping.ShippingNumber);
            }
            
        }
    }
}
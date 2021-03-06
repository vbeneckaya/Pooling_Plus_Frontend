using System;
using Application.BusinessModels.Shared.Actions;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Shippings;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Создать перевозку
    /// </summary>
    [ActionGroup(nameof(Order)), OrderNumber(2)]
    public class CreateShipping : IAppAction<Order>
    {
        private readonly IHistoryService _historyService;
        private readonly ICommonDataService _dataService;
        private readonly IShippingCalculationService _shippingCalculationService;
        private readonly IShippingGetRouteService _shippingGetRouteService;
        private readonly IChangeTrackerFactory _changeTrackerFactory;

        public CreateShipping(ICommonDataService dataService, 
                              IHistoryService historyService,
                              IShippingCalculationService shippingCalculationService,
                              IShippingGetRouteService shippingGetRouteService,
                              IChangeTrackerFactory changeTrackerFactory)
        {
            _dataService = dataService;
            _historyService = historyService;
            _shippingCalculationService = shippingCalculationService;
            _shippingGetRouteService = shippingGetRouteService;
            _changeTrackerFactory = changeTrackerFactory;
            Color = AppColor.Blue;
            Description = "Создать одну перевозку для одной накладной";
        }

        public AppColor Color { get; set; }
        public string Description { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            var unionOrders = new UnionOrders(_dataService, _historyService, _shippingCalculationService, _changeTrackerFactory, _shippingGetRouteService);
            return unionOrders.Run(user, new[] { order });
        }

        public bool IsAvailable(Order order)
        {
            return order.Status == OrderState.Created && order.OrderNumber != null
                                                      && order.DeliveryWarehouseId.HasValue && order.DeliveryWarehouseId.Value != Guid.Empty
                                                      && order.ShippingWarehouseId.HasValue && order.ShippingWarehouseId.Value != Guid.Empty
                                                      && order.ShippingDate.HasValue
                                                      && order.DeliveryDate.HasValue
                                                      && (order.BoxesCount.HasValue && order.BoxesCount.Value >0 || order.PalletsCount.HasValue && order.PalletsCount.Value >0)
                                                      && order.WeightKg.HasValue && order.WeightKg.Value >0
                ;
        }
    }
}
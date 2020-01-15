using Application.BusinessModels.Shared.Actions;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
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
        private readonly IShippingTarifficationTypeDeterminer _shippingTarifficationTypeDeterminer;
        private readonly IShippingCalculationService _shippingCalculationService;
        private readonly IChangeTrackerFactory _changeTrackerFactory;

        public CreateShipping(ICommonDataService dataService, 
                              IHistoryService historyService,
                              IShippingTarifficationTypeDeterminer shippingTarifficationTypeDeterminer,
                              IShippingCalculationService shippingCalculationService,
                              IChangeTrackerFactory changeTrackerFactory)
        {
            _dataService = dataService;
            _historyService = historyService;
            _shippingTarifficationTypeDeterminer = shippingTarifficationTypeDeterminer;
            _shippingCalculationService = shippingCalculationService;
            _changeTrackerFactory = changeTrackerFactory;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            var unionOrders = new UnionOrders(_dataService, _historyService, _shippingTarifficationTypeDeterminer, _shippingCalculationService, _changeTrackerFactory);
            return unionOrders.Run(user, new[] { order });
        }

        public bool IsAvailable(Order order)
        {
            return order.Status == OrderState.Created && (!order.DeliveryType.HasValue || order.DeliveryType.Value == DeliveryType.Delivery);
        }
    }
}
using Application.BusinessModels.Shared.Actions;
using Application.Services.Shippings;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Linq;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Убрать из перевозки
    /// </summary>
    public class RemoveFromShipping : IAppAction<Order>
    {
        private readonly IHistoryService _historyService;
        private readonly ICommonDataService _dataService;
        private readonly IShippingCalculationService _shippingCalculationService;
        private readonly IChangeTrackerFactory _changeTrackerFactory;

        public RemoveFromShipping(ICommonDataService dataService, 
                                  IHistoryService historyService, 
                                  IShippingCalculationService shippingCalculationService,
                                  IChangeTrackerFactory changeTrackerFactory)
        {
            _dataService = dataService;
            _historyService = historyService;
            _shippingCalculationService = shippingCalculationService;
            _changeTrackerFactory = changeTrackerFactory;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            order.Status = OrderState.Created;
            order.ShippingStatus = VehicleState.VehicleEmpty;
            order.DeliveryStatus = VehicleState.VehicleEmpty;

            var shipping = _dataService.GetById<Shipping>(order.ShippingId.Value);

            order.ShippingId = null;
            order.ShippingNumber = null;
            order.OrderShippingStatus = null;

            _historyService.Save(order.Id, "orderRemovedFromShipping", order.OrderNumber, shipping.ShippingNumber);

            var orders = _dataService.GetDbSet<Order>().Where(x => x.ShippingId == shipping.Id && x.Id != order.Id).ToList();
            if (!orders.Any())
            {
                shipping.Status = ShippingState.ShippingCanceled;
                _historyService.Save(shipping.Id, "shippingSetCancelled", shipping.ShippingNumber);
            }
            else
            {
                _historyService.Save(shipping.Id, "orderRemovedFromShipping", order.OrderNumber, shipping.ShippingNumber);

                _shippingCalculationService.RecalculateShipping(shipping, orders);

                var changes = _dataService.GetChanges<Shipping>().FirstOrDefault(x => x.Entity.Id == shipping.Id);
                var changeTracker = _changeTrackerFactory.CreateChangeTracker();
                changeTracker.LogTrackedChanges(changes);
            }
            
            return new AppActionResult
            {
                IsError = false,
                Message = "orderRemovedFromShipping".Translate(user.Language, order.OrderNumber, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Order order)
        {
            return order.Status == OrderState.InShipping;
        }
    }
}
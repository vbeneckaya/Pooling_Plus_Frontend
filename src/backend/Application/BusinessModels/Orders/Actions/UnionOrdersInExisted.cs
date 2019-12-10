using Application.BusinessModels.Shared.Actions;
using Application.Services.Shippings;
using DAL.Queries;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Collections.Generic;
using System.Linq;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Объеденить заказы в существующую
    /// </summary>
    public class UnionOrdersInExisted : UnionOrdersBase, IGroupAppAction<Order>
    {
        private readonly IHistoryService _historyService;
        private readonly IChangeTrackerFactory _changeTrackerFactory;

        public UnionOrdersInExisted(ICommonDataService dataService, 
                                    IHistoryService historyService, 
                                    IShippingCalculationService shippingCalculationService,
                                    IChangeTrackerFactory changeTrackerFactory)
            : base(dataService, shippingCalculationService)
        {
            _historyService = historyService;
            _changeTrackerFactory = changeTrackerFactory;
            Color = AppColor.Orange;
        }
        
        public AppColor Color { get; set; }
        public AppActionResult Run(CurrentUserDto user, IEnumerable<Order> orders)
        {
            var shippingId = orders.Single(x => x.Status == OrderState.InShipping).ShippingId;

            orders = orders.Where(x => x.Status == OrderState.Confirmed);

            var shippingDbSet = _dataService.GetDbSet<Shipping>();
            var shipping = shippingDbSet.GetById(shippingId.Value);

            if (shipping.Status == ShippingState.ShippingConfirmed)
            {
                shipping.Status = ShippingState.ShippingRequestSent;

                string orderNumbers = string.Join(", ", orders.Select(x => x.OrderNumber));
                _historyService.Save(shipping.Id, "shippingAddOrdersResendRequest", shipping.ShippingNumber, orderNumbers);
            }

            var allOrders = _dataService.GetDbSet<Order>().Where(x => x.ShippingId == shipping.Id).ToList();
            allOrders.AddRange(orders);
            
            UnionOrderInShipping(allOrders, orders, shipping, _historyService);

            var changes = _dataService.GetChanges<Shipping>().FirstOrDefault(x => x.Entity.Id == shipping.Id);
            var changeTracker = _changeTrackerFactory.CreateChangeTracker();
            changeTracker.LogTrackedChanges(changes);

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetCreated".Translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(IEnumerable<Order> target)
        {
            return target.Count() > 1 && 
                   target.Count(x => x.Status == OrderState.InShipping) == 1 &&
                   target.All(x => x.Status == OrderState.InShipping || x.Status == OrderState.Confirmed && (!x.DeliveryType.HasValue || x.DeliveryType.Value == DeliveryType.Delivery));
        }
    }
}
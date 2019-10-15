using System.Linq;
using Application.Shared;
using DAL;
using DAL.Queries;
using DAL.Services;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Убрать из перевозки
    /// </summary>
    public class RemoveFromShipping : IAppAction<Order>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService dataService;

        public RemoveFromShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            this.dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            var setter = new FieldSetter<Order>(order, _historyService);

            setter.UpdateField(o => o.Status, OrderState.Created, ignoreChanges: true);
            setter.UpdateField(o => o.ShippingStatus, VehicleState.VehicleEmpty);
            setter.UpdateField(o => o.DeliveryStatus, VehicleState.VehicleEmpty);

            var shipping = dataService.GetById<Shipping>(order.ShippingId.Value);

            order.ShippingId = null;
            order.ShippingNumber = null;

            _historyService.Save(order.Id, "orderRemovedFromShipping", order.OrderNumber, shipping.ShippingNumber);
            setter.SaveHistoryLog();

            if (dataService.GetDbSet<Order>().Any(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id))
            {
                shipping.Status = ShippingState.ShippingCanceled;
                _historyService.Save(shipping.Id, "shippingSetCancelled", shipping.ShippingNumber);
            }
            else
            {
                _historyService.Save(shipping.Id, "orderRemovedFromShipping", order.OrderNumber, shipping.ShippingNumber);
            }
            
            
            dataService.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = "orderRemovedFromShipping".translate(user.Language, order.OrderNumber, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.InShipping && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
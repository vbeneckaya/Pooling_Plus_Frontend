using System;
using System.Linq;
using Application.Shared;
using DAL;
using DAL.Queries;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Убрать из перевозки
    /// </summary>
    public class RemoveFromShipping : IAppAction<Order>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public RemoveFromShipping(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            var setter = new FieldSetter<Order>(order, _historyService);

            setter.UpdateField(o => o.Status, OrderState.Created, ignoreChanges: true);
            setter.UpdateField(o => o.ShippingStatus, VehicleState.VehicleEmpty);
            setter.UpdateField(o => o.DeliveryStatus, VehicleState.VehicleEmpty);

            var shipping = db.Shippings.GetById(order.ShippingId.Value);

            order.ShippingId = null;

            _historyService.Save(order.Id, "orderRemovedFromShipping", order.OrderNumber, shipping.ShippingNumber);
            setter.SaveHistoryLog();

            if (db.Orders.Any(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id))
            {
                shipping.Status = ShippingState.ShippingCanceled;
                _historyService.Save(shipping.Id, "shippingSetCancelled", shipping.ShippingNumber);
            }
            else
            {
                _historyService.Save(shipping.Id, "orderRemovedFromShipping", order.OrderNumber, shipping.ShippingNumber);
            }
            
            
            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Заказ убран из перевозки {order.OrderNumber}"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.InShipping && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
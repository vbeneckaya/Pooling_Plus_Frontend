using System;
using System.Linq;
using DAL;
using DAL.Queries;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Убрать из перевозки
    /// </summary>
    public class RemoveFromShipping : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public RemoveFromShipping(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            order.Status = OrderState.Created;
            order.ShippingStatus = VehicleState.VehicleEmpty;
            order.DeliveryStatus = VehicleState.VehicleEmpty;

            var shipping = db.Shippings.GetById(order.ShippingId.Value);
            
            if (db.Orders.Any(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id))
                shipping.Status = ShippingState.ShippingCanceled;

            order.ShippingId = null;
            
            
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
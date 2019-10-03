using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Объеденить заказы
    /// </summary>
    public class UnionOrders : IGroupAppAction<Order>
    {
        private readonly AppDbContext db;

        public UnionOrders(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Orange;
        }
        
        public AppColor Color { get; set; }
        public AppActionResult Run(User user, IEnumerable<Order> orders)
        {
            var shippingsCount = db.Shippings.Count();
            var shipping = new Shipping
            {
                Status = ShippingState.ShippingCreated,
                Id = Guid.NewGuid(),
                ShippingNumber = string.Format("SH{0:000000}", shippingsCount + 1),
                DeliveryType = DeliveryType.Delivery
            };
            db.Shippings.Add(shipping);

            foreach (var order in orders)
            {
                order.Status = OrderState.InShipping;
                order.ShippingId = shipping.Id;
                order.ShippingStatus = VehicleState.VehicleWaiting;
                order.DeliveryStatus = VehicleState.VehicleEmpty;
            }
            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = $"Созданна перевозка {shipping.ShippingNumber}"
            };
        }

        public bool IsAvailable(Role role, IEnumerable<Order> target)
        {
            return target.All(entity => entity.Status == OrderState.Created && (role.Name == "Administrator" || role.Name == "TransportCoordinator"));
        }
    }
}
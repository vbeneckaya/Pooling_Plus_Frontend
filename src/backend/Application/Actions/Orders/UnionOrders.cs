using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
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
            var shipping = new Shipping
            {
                Status = ShippingState.ShippingCreated
            };
            db.Shippings.Add(shipping);

            foreach (var order in orders)
            {
                order.Status = OrderState.InShipping;
                order.ShippingId = shipping.Id;
            }
            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = $"Созданна перевозка {shipping.Id}"
            };
        }

        public bool IsAvailable(Role role, IEnumerable<Order> target)
        {
            return target.All(entity => entity.Status == OrderState.Created && (role.Name == "Administrator" || role.Name == "TransportCoordinator"));
        }
    }
}
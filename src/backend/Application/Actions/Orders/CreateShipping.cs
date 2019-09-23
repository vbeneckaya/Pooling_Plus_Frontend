using System;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    public class CreateShipping : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public CreateShipping(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            var shipping = new Shipping
            {
                Status = ShippingState.Created,
                Id = Guid.NewGuid()
            };
            db.Shippings.Add(shipping);

            order.Status = OrderState.InShipping;
            order.ShippingId = shipping.Id;
            
            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Созданна перевозка {shipping.Id}"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Created && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
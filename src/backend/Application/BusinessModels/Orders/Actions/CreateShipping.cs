using System;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Создать перевозку
    /// </summary>
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
            var currentNumber = !db.Shippings.Any() ? 0 : db.Shippings.Max(x => x.TransportationNumber);
            
            var shipping = new Shipping
            {
                Status = ShippingState.ShippingCreated,
                Id = Guid.NewGuid(),
                TransportationNumber = currentNumber + 1,
                DeliveryMethod = "Доставка"
            };
            db.Shippings.Add(shipping);

            order.Status = OrderState.InShipping;
            order.ShippingId = shipping.Id;
            order.ShippingStatus = VehicleState.VehicleWaiting;
            order.DeliveryStatus = VehicleState.VehicleEmpty;

            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Созданна перевозка {shipping.TransportationNumber}"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return order.Status == OrderState.Created && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
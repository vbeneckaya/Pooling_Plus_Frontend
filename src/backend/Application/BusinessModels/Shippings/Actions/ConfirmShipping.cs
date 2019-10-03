using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using System.Linq;

namespace Application.BusinessModels.Shippings.Actions
{
    public class ConfirmShipping : IAppAction<Shipping>
    {
        private readonly AppDbContext db;
        public AppColor Color { get; set; }

        public ConfirmShipping(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Green;
        }
        public AppActionResult Run(User user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingConfirmed;

            var orders = db.Orders.Where(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id).ToList();
            foreach (Order order in orders)
            {
                order.ShippingStatus = VehicleState.VehicleWaiting;
            }

            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = $"Перевозка {shipping.ShippingNumber} подтверждена"
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return shipping.Status == ShippingState.ShippingRequestSent && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}

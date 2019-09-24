using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Shippings
{
    public class CancelShipping : IAppAction<Shipping>
    {
        private readonly AppDbContext db;
        public AppColor Color { get; set; }

        public CancelShipping(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Red;
        }
        public AppActionResult Run(User user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingCanceled;
            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = $"Перевозка {shipping.TransportationNumber} отменена"
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return (shipping.Status == ShippingState.ShippingCreated) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
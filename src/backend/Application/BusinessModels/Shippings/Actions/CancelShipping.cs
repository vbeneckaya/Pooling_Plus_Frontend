using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.BusinessModels.Shippings.Actions
{
    /// <summary>
    /// Отменить перевозку
    /// </summary>
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

            var orders = db.Orders.Where(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id).ToList();
            
            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = $"Перевозка {shipping.ShippingNumber} отменена"
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return (shipping.Status == ShippingState.ShippingCreated || shipping.Status == ShippingState.ShippingRequestSent) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
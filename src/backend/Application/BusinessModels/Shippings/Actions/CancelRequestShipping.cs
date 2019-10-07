using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.BusinessModels.Shippings.Actions
{
    /// <summary>
    /// Отменить заявку
    /// </summary>
    public class CancelRequestShipping : IAppAction<Shipping>
    {
        private readonly AppDbContext db;
        public AppColor Color { get; set; }

        public CancelRequestShipping(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Red;
        }
        public AppActionResult Run(User user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingCreated;

            var orders = db.Orders.Where(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id).ToList();
            foreach (var order in orders)
            {
                order.ShippingId = null;
            }

            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = $"Перевозка {shipping.ShippingNumber} отменена. Заказы"
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return (shipping.Status == ShippingState.ShippingCreated || shipping.Status == ShippingState.ShippingRequestSent) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
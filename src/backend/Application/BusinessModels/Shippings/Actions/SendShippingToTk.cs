using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.BusinessModels.Shippings.Actions
{
    /// <summary>
    /// Отправить перевозку в ТК
    /// </summary>
    public class SendShippingToTk : IAppAction<Shipping>
    {
        private readonly AppDbContext db;
        public AppColor Color { get; set; }

        public SendShippingToTk(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Blue;
        }
        public AppActionResult Run(User user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingRequestSent;
            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = $"Перевозка {shipping.ShippingNumber} отправлена в ТК"
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return (shipping.Status == ShippingState.ShippingCreated) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
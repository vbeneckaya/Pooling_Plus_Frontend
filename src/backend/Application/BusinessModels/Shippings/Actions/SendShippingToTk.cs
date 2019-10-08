using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;

namespace Application.BusinessModels.Shippings.Actions
{
    /// <summary>
    /// Отправить перевозку в ТК
    /// </summary>
    public class SendShippingToTk : IAppAction<Shipping>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public SendShippingToTk(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Blue;
        }

        public AppActionResult Run(User user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingRequestSent;

            _historyService.Save(shipping.Id, "shippingSetRequestSent", shipping.ShippingNumber);

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
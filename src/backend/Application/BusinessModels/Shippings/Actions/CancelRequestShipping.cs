using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;

namespace Application.BusinessModels.Shippings.Actions
{
    /// <summary>
    /// Отменить заявку
    /// </summary>
    public class CancelRequestShipping : IAppAction<Shipping>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public CancelRequestShipping(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Red;
        }
        public AppActionResult Run(User user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingCreated;

            _historyService.Save(shipping.Id, "shippingSetCancelledRequest", shipping.ShippingNumber);

            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = $"Заявка на перевозку {shipping.ShippingNumber} отменена"
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return (shipping.Status == ShippingState.ShippingRequestSent) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
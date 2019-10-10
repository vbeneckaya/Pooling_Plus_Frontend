using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Shippings.Actions
{
    /// <summary>
    /// Отменить заявку
    /// </summary>
    public class RejectRequestShipping : IAppAction<Shipping>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public RejectRequestShipping(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Red;
        }
        public AppActionResult Run(CurrentUserDto user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingRejectedByTc;

            _historyService.Save(shipping.Id, "shippingSetRejected", shipping.ShippingNumber);

            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetRejected".translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return (shipping.Status == ShippingState.ShippingRequestSent) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
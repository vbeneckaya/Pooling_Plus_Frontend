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
    /// Отменить перевозку
    /// </summary>
    public class CancelShipping : IAppAction<Shipping>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public CancelShipping(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Red;
        }

        public AppActionResult Run(CurrentUserDto user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingCanceled;

            _historyService.Save(shipping.Id, "shippingSetCancelled", shipping.ShippingNumber);

            var orders = db.Orders.Where(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id).ToList();
            foreach (var order in orders)
            {
                order.ShippingId = null;
                _historyService.Save(order.Id, "orderCancellingShipping", order.OrderNumber, shipping.ShippingNumber);
            }

            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetCancelled".translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return (shipping.Status == ShippingState.ShippingCreated || shipping.Status == ShippingState.ShippingRequestSent || shipping.Status == ShippingState.ShippingConfirmed) 
                && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
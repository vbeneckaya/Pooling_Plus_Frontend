using Application.Shared;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Linq;

namespace Application.BusinessModels.Shippings.Actions
{
    public class ConfirmShipping : IAppAction<Shipping>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public ConfirmShipping(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Green;
        }

        public AppActionResult Run(CurrentUserDto user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingConfirmed;

            var orders = db.Orders.Where(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id).ToList();
            foreach (Order order in orders)
            {
                var setter = new FieldSetter<Order>(order, _historyService);
                setter.UpdateField(o => o.ShippingStatus, VehicleState.VehicleWaiting);
                setter.SaveHistoryLog();
            }

            _historyService.Save(shipping.Id, "shippingSetConfirmed", shipping.ShippingNumber);

            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetConfirmed".translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return shipping.Status == ShippingState.ShippingRequestSent && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}

using Application.Shared;
using DAL;
using DAL.Services;
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
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public ConfirmShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            this._dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Green;
        }

        public AppActionResult Run(CurrentUserDto user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingConfirmed;

            var orders = _dataService.GetDbSet<Order>().Where(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id).ToList();
            foreach (Order order in orders)
            {
                var setter = new FieldSetter<Order>(order, _historyService);
                setter.UpdateField(o => o.ShippingStatus, VehicleState.VehicleWaiting);
                setter.SaveHistoryLog();
            }

            _historyService.Save(shipping.Id, "shippingSetConfirmed", shipping.ShippingNumber);

            _dataService.SaveChanges();

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

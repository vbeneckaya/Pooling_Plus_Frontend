using Application.BusinessModels.Shared.Actions;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Linq;

namespace Application.BusinessModels.Shippings.Actions
{
    /// <summary>
    /// Отменить перевозку
    /// </summary>
    public class CancelShipping : IAppAction<Shipping>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public CancelShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Red;
        }

        public AppActionResult Run(CurrentUserDto user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingCanceled;

            _historyService.Save(shipping.Id, "shippingSetCancelled", shipping.ShippingNumber);

            var orders = _dataService.GetDbSet<Order>().Where(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id).ToList();
            foreach (var order in orders)
            {
                order.ShippingId = null;
                order.ShippingNumber = null;
                order.OrderShippingStatus = null;

                _historyService.Save(order.Id, "orderCancellingShipping", order.OrderNumber, shipping.ShippingNumber);
            }

            _dataService.SaveChanges();

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
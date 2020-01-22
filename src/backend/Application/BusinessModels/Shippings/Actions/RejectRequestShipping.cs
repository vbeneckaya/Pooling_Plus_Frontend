using System.Linq;
using Application.BusinessModels.Shared.Actions;
using DAL.Services;
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
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }
        public string Description { get; set; }

        public RejectRequestShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Red;
            Description = "Отменить";
        }
        public AppActionResult Run(CurrentUserDto user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingRejectedByTc;

            foreach (var order in _dataService.GetDbSet<Order>().Where(o => o.ShippingId == shipping.Id))
            {
                order.OrderShippingStatus = shipping.Status;
            }

            _historyService.Save(shipping.Id, "shippingSetRejected", shipping.ShippingNumber);

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetRejected".Translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Shipping shipping)
        {
            return IsAvailable(shipping.Status);
        }

        public bool IsAvailable(ShippingState? shippingStatus)
        {
            return shippingStatus == ShippingState.ShippingRequestSent;
        }
    }
}
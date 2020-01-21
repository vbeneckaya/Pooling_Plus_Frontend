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
    /// Отправить перевозку в ТК
    /// </summary>
    public class SendShippingToTk : IAppAction<Shipping>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public SendShippingToTk(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Blue;
        }

        public AppActionResult Run(CurrentUserDto user, Shipping shipping)
        {
            if (shipping?.CompanyId == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = "shippingDontSetRequestSentDontSetTk".Translate(user.Language, shipping.ShippingNumber)
                };
            shipping.Status = ShippingState.ShippingRequestSent;

            foreach (var order in _dataService.GetDbSet<Order>().Where(o => o.ShippingId == shipping.Id))
            {
                //if(order.)
                order.OrderShippingStatus = shipping.Status;
            }

            _historyService.Save(shipping.Id, "shippingSetRequestSent", shipping.ShippingNumber);

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetRequestSent".Translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Shipping shipping)
        {
            return IsAvailable(shipping.Status);
        }

        public bool IsAvailable(ShippingState? shippingStatus)
        {
            return shippingStatus == ShippingState.ShippingCreated ||
                   shippingStatus == ShippingState.ShippingRejectedByTc;
        }
    }
}
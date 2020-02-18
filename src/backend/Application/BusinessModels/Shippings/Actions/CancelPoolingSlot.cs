using System.Linq;
using Application.BusinessModels.Shared.Actions;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Integrations.Pooling;

namespace Application.BusinessModels.Shippings.Actions
{
    public class CancelPoolingSlot : IAppAction<Shipping>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }
        public string Description { get; set; }

        public CancelPoolingSlot(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Red;
        }

        public AppActionResult Run(CurrentUserDto userDto, Shipping shipping)
        {
            var user = _dataService.GetById<User>(userDto.Id.Value);

            if (!user.IsPoolingIntegrated())
                return new AppActionResult
                {
                    IsError = true,
                    Message = "Укажите данные для доступа к pooling.me в настройках профиля"
                };
            
            using (var pooling = new PoolingIntegration(user, _dataService))
            {
                pooling.CancelReservation(shipping);
                var poolingInfoDto = pooling.GetInfoFor(shipping);
                shipping.PoolingInfo = poolingInfoDto.MessageField;
            }

            shipping.PoolingSlotId = null;
            shipping.PoolingReservationId = null;
            shipping.Status = ShippingState.ShippingCreated;
            shipping.PoolingState = ShippingPoolingState.PoolingAvailable;

            foreach (var order in _dataService.GetDbSet<Order>().Where(o => o.ShippingId == shipping.Id))
            {
                order.OrderShippingStatus = shipping.Status;
            }

            _historyService.Save(shipping.Id, "shippingDeletePoolingSlot", shipping.ShippingNumber);

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingDeletePoolingSlot".Translate(userDto.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Shipping shipping)
        {
            return shipping.PoolingState == ShippingPoolingState.PoolingBooked;
        }
    }
}
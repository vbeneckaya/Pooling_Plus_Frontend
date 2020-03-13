using Application.BusinessModels.Shared.Actions;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Linq;
using Integrations.Pooling;

namespace Application.BusinessModels.Shippings.Actions
{
    public class SendToPooling : IAppAction<Shipping>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }
        public string Description { get; set; }

        public SendToPooling(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Teal;
            Description = "Забронировать слот на пулинге";
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
                var poolingInfo = pooling.GetInfoFor(shipping);
                if (poolingInfo.IsAvailable)
                {
                    var reservationResult = pooling.CreateReservation(shipping);
                    if (!string.IsNullOrEmpty(reservationResult.Error))
                    {
                        return new AppActionResult
                        {
                            IsError = true,
                            Message = $"Pooling: {reservationResult.Error}"
                        };
                    }

                    shipping.PoolingInfo = $"Номер брони на Pooling: {reservationResult.ReservationNumber}";
                    shipping.PoolingSlotId = poolingInfo.SlotId;
                    shipping.PoolingReservationId = reservationResult.ReservationId;
                }
                else
                {
                    shipping.PoolingState = null;
                    return new AppActionResult
                    {
                        IsError = true,
                        Message = "Бронирование не доступно"
                    };
                }
            }

            shipping.Status = ShippingState.ShippingConfirmed;
            shipping.PoolingState = ShippingPoolingState.PoolingBooked;

            foreach (var order in _dataService.GetDbSet<Order>().Where(o => o.ShippingId == shipping.Id))
            {
                order.OrderShippingStatus = shipping.Status;
            }

            _historyService.Save(shipping.Id, "shippingSetPooling", shipping.ShippingNumber);

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetPooling".Translate(userDto.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Shipping shipping)
        {
            return shipping.PoolingState == ShippingPoolingState.PoolingAvailable;
        }
    }
}
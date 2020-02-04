using System;
using Application.BusinessModels.Shared.Actions;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Linq;
using Integrations;
using Integrations.Dtos;
using Integrations.Pooling;
using Integrations.Pooling.Dtos;

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

            try
            {
                using (var pooling = new PoolingIntegration(user, _dataService))
                {
                    if (pooling.IsAvaliable(shipping))
                    {
                        var reservationNumber = pooling.CreateReservation(shipping);
                        shipping.PoolingInfo = $"Номер брони на Pooling: {reservationNumber}";
                    }
                }
            }
            catch (Exception e)
            {
                return new AppActionResult
                {
                    IsError = true,
                    Message = e.Message
                };
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
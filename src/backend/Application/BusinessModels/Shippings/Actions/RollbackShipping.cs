using Application.BusinessModels.Shared.Actions;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Linq;

namespace Application.BusinessModels.Shippings.Actions
{
    /// <summary>
    /// Вернуть в предыдущий статус
    /// </summary>
    public class RollbackShipping : IAppAction<Shipping>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService _dataService;
        public string Description { get; set; }

        public RollbackShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Grey;
            Description = "Переместить перевозку в предыдущий статус";
        }
        
        public AppColor Color { get; set; }
        public AppActionResult Run(CurrentUserDto user, Shipping shipping)
        {
            var newState = new ShippingState?();
            
            if (shipping.Status == ShippingState.ShippingCompleted)
                newState = ShippingState.ShippingConfirmed;

            if (shipping.Status == ShippingState.ShippingBillSend)
                newState = ShippingState.ShippingCompleted;

            if (shipping.Status == ShippingState.ShippingArhive)
                newState = ShippingState.ShippingBillSend;

            
            if (newState.HasValue)
            {
                shipping.Status = newState.Value;

                foreach (var order in _dataService.GetDbSet<Order>().Where(o => o.ShippingId == shipping.Id))
                {
                    order.OrderShippingStatus = shipping.Status;
                }

                _historyService.Save(shipping.Id, "shippingRollback", 
                    shipping.ShippingNumber, 
                    newState.ToString().ToLowerFirstLetter());
            }

            string newStateName = newState?.ToString()?.ToLowerFirstLetter().Translate(user.Language);
            return new AppActionResult
            {
                IsError = false,
                Message = "shippingRollback".Translate(user.Language, 
                    shipping.ShippingNumber,
                    newStateName)
            };
        }

        public bool IsAvailable(Shipping shipping)
        {
            return IsAvailable(shipping.Status);
        }

        public bool IsAvailable(ShippingState? shippingStatus)
        {
            return shippingStatus == ShippingState.ShippingCompleted ||
                   shippingStatus == ShippingState.ShippingBillSend ||
                   shippingStatus == ShippingState.ShippingArhive;
        }
    }
}
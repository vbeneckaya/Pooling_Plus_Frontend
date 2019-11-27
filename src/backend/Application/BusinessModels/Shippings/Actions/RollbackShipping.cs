using Application.BusinessModels.Shared.Actions;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Shippings.Actions
{
    /// <summary>
    /// Вернуть в предыдущий статус
    /// </summary>
    public class RollbackShipping : IAppAction<Shipping>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService _dataService;

        public RollbackShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Grey;
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
                
                _historyService.Save(shipping.Id, "shippingRollback", 
                    shipping.ShippingNumber, 
                    newState.ToString().ToLowerFirstLetter());
            }
            
            return new AppActionResult
            {
                IsError = false,
                Message = "shippingRollback".Translate(user.Language, 
                    shipping.ShippingNumber, 
                    newState.ToString().ToLowerFirstLetter())
            };
        }

        public bool IsAvailable(Shipping shipping)
        {
            return shipping.Status == ShippingState.ShippingCompleted ||
                   shipping.Status == ShippingState.ShippingBillSend ||
                   shipping.Status == ShippingState.ShippingArhive;
        }
    }
}
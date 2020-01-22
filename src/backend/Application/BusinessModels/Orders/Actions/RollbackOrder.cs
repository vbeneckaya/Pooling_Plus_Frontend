using Application.BusinessModels.Shared.Actions;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Вернуть в предыдущий статус
    /// </summary>
    [ActionGroup(nameof(Order)), OrderNumber(13)]
    public class RollbackOrder : IAppAction<Order>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService _dataService;

        public RollbackOrder(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Grey;
            Description = "Вернуть накладную в предыдущий статус. Статус перевозки не изменится";
        }
        
        public AppColor Color { get; set; }
        public string Description { get; set; }

        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            var newState = new OrderState?();
            
            if (order.Status == OrderState.Canceled){
                if (order.DeliveryWarehouseId != null &&
                    !string.IsNullOrEmpty(order.Payer) &&
                    !string.IsNullOrEmpty(order.DeliveryAddress) &&
                    !string.IsNullOrEmpty(order.ShippingAddress) &&
                    order.ShippingDate != null && 
                    (order.DeliveryType == null || order.DeliveryType == DeliveryType.Delivery) && order.DeliveryDate != null ||
                    order.DeliveryType == DeliveryType.SelfDelivery)
                    newState = OrderState.Created;
                else
                    newState = OrderState.Created;
            
            }
            

            if (order.Status == OrderState.Shipped)
            {
                if (!order.DeliveryType.HasValue || order.DeliveryType == DeliveryType.Delivery)
                    newState = OrderState.InShipping;
            }
            
            
            if (order.Status == OrderState.Delivered)
                newState = OrderState.Shipped;
            
            if (order.Status == OrderState.Archive)
                newState = OrderState.Delivered;
            
            if (newState.HasValue)
            {
                order.Status = newState.Value;
                
                _historyService.Save(order.Id, "orderRollback", 
                    order.OrderNumber, 
                    newState.ToString().ToLowerFirstLetter());
            }

            string newStateName = newState?.ToString()?.ToLowerFirstLetter().Translate(user.Language);
            return new AppActionResult
            {
                IsError = false,
                Message = "orderRollback".Translate(user.Language, 
                    order.OrderNumber,
                    newStateName)
            };
        }

        public bool IsAvailable(Order order)
        {
            return order.Status == OrderState.Shipped ||
                   order.Status == OrderState.Delivered ||
                   order.Status == OrderState.Canceled ||
                   order.Status == OrderState.Archive;
        }
    }
}
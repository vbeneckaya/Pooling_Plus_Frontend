using System;
using System.Collections.Generic;
using System.Linq;
using Application.BusinessModels.Shared.Actions;
using Application.Shared;
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
    public class RollbackOrder : IAppAction<Order>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService _dataService;

        public RollbackOrder(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Grey;
        }
        
        public AppColor Color { get; set; }
        public AppActionResult Run(CurrentUserDto user, Order order)
        {
            var newState = new OrderState?();
            
            if (order.Status == OrderState.Confirmed)
                newState = OrderState.Created;

            if (order.Status == OrderState.Shipped)
            {
                if (!order.DeliveryType.HasValue || order.DeliveryType == DeliveryType.Delivery)
                    newState = OrderState.InShipping;
                else
                    newState = OrderState.Confirmed;
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
            
            return new AppActionResult
            {
                IsError = false,
                Message = "orderRollback".Translate(user.Language, 
                    order.OrderNumber, 
                    newState.ToString().ToLowerFirstLetter())
            };
        }

        public bool IsAvailable(Order order)
        {
            return order.Status == OrderState.Confirmed ||
                   order.Status == OrderState.Shipped ||
                   order.Status == OrderState.Delivered ||
                   order.Status == OrderState.Archive;
        }
    }
}
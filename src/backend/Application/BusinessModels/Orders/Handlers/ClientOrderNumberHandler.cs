﻿using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.History;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ClientOrderNumberHandler : IFieldHandler<Order, string>
    {
        public void AfterChange(Order order, string oldValue, string newValue)
        {
            OrderType newOrderType;
            if (order.ClientOrderNumber.StartsWith("2"))
                newOrderType = OrderType.FD;
            else
                newOrderType = OrderType.OR;

            order.OrderType = newOrderType;
            order.OrderChangeDate = DateTime.UtcNow;
        }

        public string ValidateChange(Order order, string oldValue, string newValue)
        {
            return null;
        }

        public ClientOrderNumberHandler(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        private readonly IHistoryService _historyService;
    }
}

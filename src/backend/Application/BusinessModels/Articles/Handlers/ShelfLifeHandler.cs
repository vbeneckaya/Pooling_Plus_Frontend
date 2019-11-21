using Application.BusinessModels.Shared.Handlers;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.History;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Application.BusinessModels.Articles.Handlers
{
    public class ShelfLifeHandler : IFieldHandler<Article, int?>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public ShelfLifeHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public void AfterChange(Article entity, int? oldValue, int? newValue)
        {
            var validStatuses = new[] { OrderState.Draft, OrderState.Created, OrderState.InShipping,
                                        OrderState.Shipped, OrderState.Delivered };
            var itemsToUpdate = _dataService.GetDbSet<OrderItem>()
                                            .Include(x => x.Order)
                                            .Where(x => x.Nart == entity.Nart
                                                        && x.Order != null
                                                        && x.ShelfLife != newValue
                                                        && validStatuses.Contains(x.Order.Status))
                                            .ToList();
            foreach (var orderItem in itemsToUpdate)
            {
                orderItem.ShelfLife = newValue;
                _historyService.SaveImpersonated(null, orderItem.OrderId, "orderItemChangeShelfLife", orderItem.Nart, newValue);
            }
        }

        public string ValidateChange(Article entity, int? oldValue, int? newValue)
        {
            return null;
        }
    }
}

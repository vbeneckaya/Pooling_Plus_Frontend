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
    public class SpgrHandler : IFieldHandler<Article, string>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public SpgrHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public void AfterChange(Article entity, string oldValue, string newValue)
        {
            var validStatuses = new[] { OrderState.Draft, OrderState.Created, OrderState.InShipping };
            var itemsToUpdate = _dataService.GetDbSet<OrderItem>()
                                            .Include(x => x.Order)
                                            .Where(x => x.Nart == entity.Nart
                                                        && x.Order != null
                                                        && validStatuses.Contains(x.Order.Status)
                                                        && (x.Order.ShippingId == null || x.Order.OrderShippingStatus == ShippingState.ShippingCreated))
                                            .ToList();
            foreach (var orderItem in itemsToUpdate)
            {
                orderItem.Spgr = newValue;
                _historyService.SaveImpersonated(null, orderItem.OrderId, "orderItemChangeSpgr", orderItem.Nart, newValue);
            }
        }

        public string ValidateChange(Article entity, string oldValue, string newValue)
        {
            return null;
        }
    }
}

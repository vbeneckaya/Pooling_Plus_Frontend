using Application.BusinessModels.Shared.Actions;
using Application.Shared;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Collections.Generic;
using System.Linq;
using DAL.Queries;
using Microsoft.EntityFrameworkCore;

namespace Application.BusinessModels.Orders.Actions
{
    /// <summary>
    /// Объеденить заказы в существующую
    /// </summary>
    public class UnionOrdersInExisted : UnionOrdersBase, IGroupAppAction<Order>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService _dataService;

        public UnionOrdersInExisted(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Orange;
        }
        
        public AppColor Color { get; set; }
        public AppActionResult Run(CurrentUserDto user, IEnumerable<Order> orders)
        {
            var shippingId = orders.Single(x => x.Status == OrderState.InShipping).ShippingId;

            orders = orders.Where(x => x.Status == OrderState.Confirmed);

            var shippingDbSet = _dataService.GetDbSet<Shipping>();
            var shipping = shippingDbSet.GetById(shippingId.Value);
            
            UnionOrderInShipping(orders, shipping, shippingDbSet, _historyService);

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetCreated".Translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(IEnumerable<Order> target)
        {
            return target.Count() > 1 && 
                   target.Count(x => x.Status == OrderState.InShipping) == 1 &&
                   target.All(x => x.Status == OrderState.InShipping || x.Status == OrderState.Confirmed);
        }
    }
}
using Application.BusinessModels.Orders.Actions;
using Domain;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Orders
{
    public class OrderActionsService : IActionService<Order>
    {
        private readonly IHistoryService historyService;

        private readonly ICommonDataService dataService;

        public OrderActionsService(ICommonDataService dataService, IHistoryService historyService)
        {
            this.dataService = dataService;
            this.historyService = historyService;
        }

        public IEnumerable<IAction<Order>> GetActions()
        {
            return new List<IAction<Order>>
            {
                new CreateShipping(dataService, historyService),
                new CancelOrder(dataService, historyService),
                new RemoveFromShipping(dataService, historyService),
                new SendToArchive(dataService, historyService),
                new RecordFactOfLoss(dataService, historyService),
                new OrderShipped(dataService, historyService),
                new OrderDelivered(dataService, historyService),
                new FullReject(dataService, historyService),
                /*end of add single actions*/
            };
        }

        public IEnumerable<IAction<IEnumerable<Order>>> GetGroupActions()
        {
            return new List<IAction<IEnumerable<Order>>>
            {
                new UnionOrders(dataService, historyService),
                //new CancelOrders(dataService, historyService),
                //new CreateShippingForeach(dataService, historyService),
                /*end of add group actions*/
            };
        }
    }
}

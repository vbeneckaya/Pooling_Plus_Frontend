using Application.BusinessModels.Orders.Actions;
using Domain;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using System.Collections.Generic;

namespace Application.Services.Orders
{
    public class OrderActionsService : IActionService<Order>
    {
        private readonly IHistoryService _historyService;

        private readonly ICommonDataService _dataService;

        public OrderActionsService(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public IEnumerable<IAction<Order>> GetActions()
        {
            return new List<IAction<Order>>
            {
                new CreateShipping(_dataService, _historyService),
                new CancelOrder(_dataService, _historyService),
                new RemoveFromShipping(_dataService, _historyService),
                new SendToArchive(_dataService, _historyService),
                new RecordFactOfLoss(_dataService, _historyService),
                new OrderShipped(_dataService, _historyService),
                new OrderDelivered(_dataService, _historyService),
                new FullReject(_dataService, _historyService),
                new DeleteOrder(_dataService), 
            };
        }

        public IEnumerable<IAction<IEnumerable<Order>>> GetGroupActions()
        {
            return new List<IAction<IEnumerable<Order>>>
            {
                new UnionOrders(_dataService, _historyService),
            };
        }
    }
}

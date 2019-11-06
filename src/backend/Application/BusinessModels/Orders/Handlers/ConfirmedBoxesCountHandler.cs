using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL;
using DAL.Queries;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class ConfirmedBoxesCountHandler : IFieldHandler<Order, decimal?>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, decimal? oldValue, decimal? newValue)
        {
            if (newValue != order.BoxesCount)
            {
                var setter = new FieldSetter<Order>(order, _historyService);
                setter.UpdateField(o => o.OrderChangeDate, DateTime.Now);
                setter.SaveHistoryLog();
            }
        }

        public string ValidateChange(Order order, decimal? oldValue, decimal? newValue)
        {
            return null;
        }

        public ConfirmedBoxesCountHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }
    }
}

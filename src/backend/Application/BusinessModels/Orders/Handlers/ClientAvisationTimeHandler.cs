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
    public class ClientAvisationTimeHandler : IFieldHandler<Order, TimeSpan?>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, TimeSpan? oldValue, TimeSpan? newValue)
        {
            var setter = new FieldSetter<Order>(order, _historyService);
            setter.UpdateField(o => o.OrderChangeDate, DateTime.Now);
            setter.SaveHistoryLog();
        }

        public string ValidateChange(Order order, TimeSpan? oldValue, TimeSpan? newValue)
        {
            return null;
        }

        public ClientAvisationTimeHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }
    }
}

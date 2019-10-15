using Application.BusinessModels.Shippings.Actions;
using DAL.Services;
using Domain;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Shippings
{
    public class ShippingActionsService : IActionService<Shipping>
    {
        private readonly IHistoryService historyService;

        private readonly ICommonDataService dataService;

        public ShippingActionsService(ICommonDataService dataService, IHistoryService historyService)
        {
            this.dataService = dataService;
            this.historyService = historyService;
        }

        public IEnumerable<IAction<Shipping>> GetActions()
        {
            return new List<IAction<Shipping>>
            {
                new SendShippingToTk(dataService, historyService),
                new ConfirmShipping(dataService, historyService),
                new RejectRequestShipping(dataService, historyService),
                new CancelRequestShipping(dataService, historyService),
                new CompleteShipping(dataService, historyService),
                new CancelShipping(dataService, historyService),
                new ProblemShipping(dataService, historyService),
                new BillingShipping(dataService, historyService),
                new ArchiveShipping(dataService, historyService),
                /*end of add single actions*/
            };
        }

        public IEnumerable<IAction<IEnumerable<Shipping>>> GetGroupActions()
        {
            return new List<IAction<IEnumerable<Shipping>>>
            {
                /*end of add group actions*/
            };
        }
    }
}

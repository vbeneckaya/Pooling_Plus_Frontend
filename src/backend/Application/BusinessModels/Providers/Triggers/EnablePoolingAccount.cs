using System;
using System.Linq;
using System.Threading.Tasks;
using Application.BackgroundServices.Provider;
using Application.BusinessModels.Shared.Triggers;
using DAL;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using Integrations.Pooling;
using Microsoft.Extensions.DependencyInjection;

namespace Application.BusinessModels.Tariffs.Triggers
{
    public class EnablePoolingAccount : ITrigger<Provider>
    {
        private readonly ICommonDataService _dataService;
        private readonly IServiceProvider _serviceProvider;

        public EnablePoolingAccount(ICommonDataService dataService, IServiceProvider serviceProvider)
        {
            _dataService = dataService;
            _serviceProvider = serviceProvider;
        }

        public void Execute(Provider provider)
        {
            if (!provider.IsPoolingIntegrated) return;


//            var dataService = _dataService;
//            var newServiceProvider = _serviceProvider;
//            
//            void Action(object obj)
//            {
//               
//                using (PoolingIntegration poolingIntegration = new PoolingIntegration(
//                    new User
//                    {
//                        PoolingLogin = "k.skvortsov@artlogics.ru",
//                        PoolingPassword = "VCuds3v"
//                    },
//                    dataService,
//                    null,
//                    newServiceProvider
//                ))
//                {
//                    var endDate = DateTime.Now.AddDays(14);
//                    var startDate = endDate.AddMonths(-3);
//                    poolingIntegration.LoadShippingsAndOrdersFromReports(startDate, endDate);
//                }
//            }
//
//            var task = Task.Factory.StartNew(Action, "poolingIntegration.LoadShippingsAndOrdersFromReports");
        }

        public bool IsTriggered(EntityChanges<Provider> changes)
        {
            var watchProperties = new[]
            {
                nameof(Provider.IsPoolingIntegrated)
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}
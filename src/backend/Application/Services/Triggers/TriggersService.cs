using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Triggers
{
    public class TriggersService : ITriggersService
    {
        private readonly ICommonDataService _dataService;
        private readonly IServiceProvider _serviceProvider;

        public TriggersService(ICommonDataService dataService, IServiceProvider serviceProvider)
        {
            _dataService = dataService;
            _serviceProvider = serviceProvider;
        }

        public void Execute()
        {
            var orderChanges = _dataService.GetChanges<Order>().ToList();
            var shippingChanges = _dataService.GetChanges<Shipping>().ToList();
            var tariffChanges = _dataService.GetChanges<Tariff>().ToList();

            _dataService.SaveChanges();

            RunTriggers(orderChanges);
            RunTriggers(shippingChanges);
            RunTriggers(tariffChanges);
        }

        private void RunTriggers<TEntity>(IEnumerable<EntityChanges<TEntity>> changes) where TEntity : class, IPersistable
        {
            var triggers = _serviceProvider.GetService<IEnumerable<ITrigger<TEntity>>>();
            if (triggers.Any())
            {
                foreach (var entityChanges in changes)
                {
                    foreach (var trigger in triggers)
                    {
                        if (trigger.IsTriggered(entityChanges))
                        {
                            trigger.Execute(entityChanges.Entity);
                        }
                    }
                }
            }
        }
    }
}

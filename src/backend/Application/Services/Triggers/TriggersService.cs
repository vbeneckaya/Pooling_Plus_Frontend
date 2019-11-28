using Application.BusinessModels.Shared.Triggers;
using DAL.Services;
using Domain.Persistables;
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
            RunTriggers<Order>();
            RunTriggers<Shipping>();
            RunTriggers<Tariff>();
        }

        private void RunTriggers<TEntity>() where TEntity : class, IPersistable
        {
            var triggers = _serviceProvider.GetService<IEnumerable<ITrigger<TEntity>>>();
            if (triggers.Any())
            {
                var changes = _dataService.GetChanges<TEntity>();
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

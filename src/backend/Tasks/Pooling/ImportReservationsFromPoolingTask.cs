using Domain.Persistables;
using Domain.Services.Orders;
using Domain.Services.ShippingWarehouses;
using Domain.Services.Warehouses;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAL.Services;
using Domain.Services.Clients;
using Domain.Services.Shippings;
using Domain.Services.TransportCompanies;
using Integrations.Pooling;
using NCrontab;
using Tasks.Common;

namespace Tasks.Pooling
{
    [Description("Импорт перевозок из Pooling")]
    public class ImportReservationsFromPoolingTask : TaskBase<ImportReservationsFromPoolingProperties>, IScheduledTask
    {
        public string Schedule => "0 */1 * * *";

        protected override async Task Execute(IServiceProvider serviceProvider,
            ImportReservationsFromPoolingProperties props, CancellationToken cancellationToken)
        {
            try
            {
                ICommonDataService dataService = serviceProvider.GetService<ICommonDataService>();
                using (PoolingIntegration poolingIntegration = new PoolingIntegration(
                    new User
                    {
                        PoolingLogin = "k.skvortsov@artlogics.ru",
                        PoolingPassword = "VCuds3v"
                    },
                    dataService,
                    serviceProvider
                ))
                {
                    var endDate = DateTime.Now;
                    var startDate = endDate - (CrontabSchedule.Parse(Schedule).GetNextOccurrence(endDate) - endDate);
                    poolingIntegration.LoadShippingsAndOrdersFromReports(startDate, endDate);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Ошибка при обработке {TaskName} инжекции.");
                throw ex;
            }
        }
    }
}
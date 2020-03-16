using Domain.Persistables;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using DAL.Services;
using Integrations.Pooling;
using Tasks.Common;

namespace Tasks.Pooling
{
    [Description("Импорт перевозок из Pooling")]
    public class ImportReservationsFromPoolingTask : TaskBase<ImportReservationsFromPoolingProperties>, IScheduledTask
    {
        public string Schedule => "* */3 * * *";

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
                    var startDate = DateTime.Now.AddDays(-14);
                    var endDate = DateTime.Now.AddDays(14);
//                    var startDate = endDate - (CrontabSchedule.Parse(Schedule).GetNextOccurrence(endDate) - endDate);
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
using Domain.Persistables;
using Domain.Services.Orders;
using Domain.Services.ShippingWarehouses;
using Domain.Services.Warehouses;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using DAL.Services;
using Domain.Services.Clients;
using Domain.Services.Shippings;
using Domain.Services.TransportCompanies;
using Integrations.Pooling;
using Tasks.Common;

namespace Tasks.Pooling
{
    [Description("Импорт перевозок из Pooling")]
    public class ImportReservationsFromPoolingTask : TaskBase<ImportReservationsFromPoolingProperties>, IScheduledTask
    {
        public string Schedule => "*/1 * * * *";

        protected override async Task Execute(IServiceProvider serviceProvider, ImportReservationsFromPoolingProperties props, CancellationToken cancellationToken)
        {
            try
            {
                //Regex fileNameRe = new Regex(props.FileNamePattern, RegexOptions.IgnoreCase);
                //IInjectionsService injectionsService = serviceProvider.GetService<IInjectionsService>();
                ICommonDataService dataService = serviceProvider.GetService<ICommonDataService>();
                IShippingWarehousesService shippingWarehousesService = serviceProvider.GetService<IShippingWarehousesService>();
                IWarehousesService warehousesService = serviceProvider.GetService<IWarehousesService>();
                IShippingsService shippingsService = serviceProvider.GetService<IShippingsService>();
                IOrdersService ordersService = serviceProvider.GetService<IOrdersService>();
                IClientsService clientsService = serviceProvider.GetService<IClientsService>();
                ITransportCompaniesService transportCompaniesService = serviceProvider.GetService<ITransportCompaniesService>();
                
                PoolingIntegration poolingIntegration = new PoolingIntegration(
                    new User
                    {
                        PoolingLogin = "k.skvortsov@artlogics.ru",
                        PoolingPassword = "Pooling"
                    }, 
                    dataService,
                    shippingWarehousesService, 
                    shippingsService, 
                    ordersService,
                    warehousesService,
                    clientsService,
                    transportCompaniesService
                    );
               poolingIntegration.LoadShippingsAndOrdersFromReports(); 
               
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Ошибка при обработке {TaskName} инжекции.");
                throw ex;
            }
        }
    }
}

﻿using Domain.Services.UserProvider;
using Infrastructure.Installers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using Tasks.Common;
using Tasks.MasterData;
using Tasks.Orders;
using Tasks.Pooling;
using Tasks.Services;

namespace Tasks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = CreateConfiguration(args);

                    services.AddDomain(configuration, false);
                    services.AddScoped<IUserProvider, TasksUserProvider>();

                    services.AddHostedService<ScheduleWorker>();

                //    services.AddScoped<IScheduledTask, ImportOrderTask>();
                //    services.AddScoped<IScheduledTask, ImportProductsTask>();
                    services.AddScoped<IScheduledTask, ImportReservationsFromPoolingTask>();
                });

        private static IConfiguration CreateConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }
    }
}
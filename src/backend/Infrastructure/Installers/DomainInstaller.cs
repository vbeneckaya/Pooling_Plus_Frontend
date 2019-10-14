using Application.Services.AppConfiguration;
/*start of using application service*/
using Application.Services.Orders;
using Application.Services.Shippings;
using Application.Services.Tariffs;
using Application.Services.Warehouses;
using Application.Services.Articles;
using Application.Services.TransportCompanies;
/*end of using application service*/
using Application.Services.Users;
using Application.Services.Identity;
using Application.Services.Roles;
using Application.Services.Translations;
using Application.Services.Injections;
using Application.Services.TaskProperties;
using DAL;
using Domain.Services.AppConfiguration;
using Domain.Services.Identity;
using Domain.Services.Roles;
using Domain.Services.Translations;
using Domain.Services.Users;
using Domain.Services.TaskProperties;
/*start of using domain service*/
using Domain.Services.Orders;
using Domain.Services.Shippings;
using Domain.Services.Tariffs;
using Domain.Services.Warehouses;
using Domain.Services.Articles;
using Domain.Services.TransportCompanies;
using Domain.Services.Injections;
/*end of using domain service*/
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Domain.Services.Files;
using Application.Services.Files;
using Domain.Services.DocumentTypes;
using Application.Services.DocumentTypes;
using Application.Services.PickingTypes;
using Domain.Services.PickingTypes;
using Application.Services.VehicleTypes;
using Domain.Services.VehicleTypes;
using Application.Services.History;
using Domain.Services.History;
using Application.Services.UserSettings;
using Domain.Services.UserSettings;

namespace Infrastructure.Installers
{
    public static class DomainInstaller
    {
        public static void AddDomain(this IServiceCollection services, IConfiguration configuration, bool migrateDb)
        {
            services.AddScoped<AppDbContext, AppDbContext>();
            services.AddScoped<IAppConfigurationService, AppConfigurationService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<ITranslationsService, TranslationsService>();
            services.AddScoped<IInjectionsService, InjectionsService>();
            services.AddScoped<ITaskPropertiesService, TaskPropertiesService>();
            services.AddScoped<IHistoryService, HistoryService>();
            services.AddScoped<IUserSettingsService, UserSettingsService>();

            /*start of add service implementation*/
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddScoped<IShippingsService, ShippingsService>();
            services.AddScoped<ITariffsService, TariffsService>();
            services.AddScoped<IWarehousesService, WarehousesService>();
            services.AddScoped<ISoldToService, SoldToService>();
            services.AddScoped<IArticlesService, ArticlesService>();
            services.AddScoped<ITransportCompaniesService, TransportCompaniesService>();
            services.AddScoped<IFilesService, FilesService>();
            services.AddScoped<IDocumentTypesService, DocumentTypesService>();
            services.AddScoped<IPickingTypesService, PickingTypesService>();
            services.AddScoped<IVehicleTypesService, VehicleTypesService>();
            /*end of add service implementation*/

            var connectionString = configuration.GetConnectionString("DefaultDatabase");

            var buildServiceProvider = services.AddEntityFrameworkNpgsql()
                .AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(connectionString);
                })
                .BuildServiceProvider();

            if (migrateDb)
            {
                var appDbContext = buildServiceProvider.GetService<AppDbContext>();
                appDbContext.Migrate(connectionString);
            }
        }
    }
}

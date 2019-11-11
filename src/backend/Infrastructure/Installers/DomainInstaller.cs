using Application.BusinessModels.Orders.Actions;
using Application.BusinessModels.Shared.Actions;
using Application.BusinessModels.Shippings.Actions;
using Application.Services.AppConfiguration;
using Application.Services.Articles;
using Application.Services.BodyTypes;
using Application.Services.Documents;
using Application.Services.DocumentTypes;
using Application.Services.FieldProperties;
using Application.Services.Files;
using Application.Services.History;
using Application.Services.Identity;
using Application.Services.Injections;
using Application.Services.Orders;
using Application.Services.PickingTypes;
using Application.Services.Roles;
using Application.Services.Shippings;
using Application.Services.ShippingWarehouses;
using Application.Services.Tariffs;
using Application.Services.TaskProperties;
using Application.Services.Tonnages;
using Application.Services.Translations;
using Application.Services.TransportCompanies;
using Application.Services.Users;
using Application.Services.UserSettings;
using Application.Services.VehicleTypes;
using Application.Services.Warehouses;
using DAL;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.AppConfiguration;
using Domain.Services.Articles;
using Domain.Services.BodyTypes;
using Domain.Services.Documents;
using Domain.Services.DocumentTypes;
using Domain.Services.FieldProperties;
using Domain.Services.Files;
using Domain.Services.History;
using Domain.Services.Identity;
using Domain.Services.Injections;
using Domain.Services.Orders;
using Domain.Services.PickingTypes;
using Domain.Services.Roles;
using Domain.Services.Shippings;
using Domain.Services.ShippingWarehouses;
using Domain.Services.Tariffs;
using Domain.Services.TaskProperties;
using Domain.Services.Tonnages;
using Domain.Services.Translations;
using Domain.Services.TransportCompanies;
using Domain.Services.Users;
using Domain.Services.UserSettings;
using Domain.Services.VehicleTypes;
using Domain.Services.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Installers
{
    public static class DomainInstaller
    {
        public static void AddDomain(this IServiceCollection services, IConfiguration configuration, bool migrateDb)
        {
            services.AddSingleton(configuration);

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

            services.AddScoped<ICommonDataService, CommonDataService>();
            services.AddScoped<IDocumentService, DocumentService>();

            /*start of add service implementation*/
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddScoped<IShippingsService, ShippingsService>();
            services.AddScoped<ITariffsService, TariffsService>();
            services.AddScoped<IShippingWarehousesService, ShippingWarehousesService>();
            services.AddScoped<IWarehousesService, WarehousesService>();
            services.AddScoped<ISoldToService, SoldToService>();
            services.AddScoped<IArticlesService, ArticlesService>();
            services.AddScoped<ITransportCompaniesService, TransportCompaniesService>();
            services.AddScoped<IFilesService, FilesService>();
            services.AddScoped<IDocumentTypesService, DocumentTypesService>();
            services.AddScoped<IPickingTypesService, PickingTypesService>();
            services.AddScoped<IVehicleTypesService, VehicleTypesService>();
            services.AddScoped<IFieldPropertiesService, FieldPropertiesService>();
            services.AddSingleton<IFieldDispatcherService, FieldDispatcherService>();
            services.AddScoped<IBodyTypesService, BodyTypesService>();
            services.AddScoped<ITonnagesService, TonnagesService>();

            /*end of add service implementation*/

            AddOrderBusinessModels(services);
            AddShippingBusinessModels(services);

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

        private static void AddOrderBusinessModels(IServiceCollection services)
        {
            services.AddScoped<IAppAction<Order>, CreateShipping>();
            services.AddScoped<IAppAction<Order>, CancelOrder>();
            services.AddScoped<IAppAction<Order>, RemoveFromShipping>();
            services.AddScoped<IAppAction<Order>, SendToArchive>();
            services.AddScoped<IAppAction<Order>, RecordFactOfLoss>();
            services.AddScoped<IAppAction<Order>, OrderShipped>();
            services.AddScoped<IAppAction<Order>, OrderDelivered>();
            services.AddScoped<IAppAction<Order>, FullReject>();
            services.AddScoped<IAppAction<Order>, DeleteOrder>();

            services.AddScoped<IGroupAppAction<Order>, UnionOrders>();
        }

        private static void AddShippingBusinessModels(IServiceCollection services)
        {
            services.AddScoped<IAppAction<Shipping>, SendShippingToTk>();
            services.AddScoped<IAppAction<Shipping>, ConfirmShipping>();
            services.AddScoped<IAppAction<Shipping>, RejectRequestShipping>();
            services.AddScoped<IAppAction<Shipping>, CancelRequestShipping>();
            services.AddScoped<IAppAction<Shipping>, CompleteShipping>();
            services.AddScoped<IAppAction<Shipping>, CancelShipping>();
            services.AddScoped<IAppAction<Shipping>, ProblemShipping>();
            services.AddScoped<IAppAction<Shipping>, BillingShipping>();
            services.AddScoped<IAppAction<Shipping>, ArchiveShipping>();
        }
    }
}

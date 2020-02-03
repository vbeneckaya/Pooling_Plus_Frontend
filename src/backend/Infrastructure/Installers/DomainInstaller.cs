using System.Linq;
using Application.BusinessModels.Orders.Actions;
using Application.BusinessModels.Orders.Triggers;
using Application.BusinessModels.Shared.Actions;
using Application.BusinessModels.Shared.Handlers;
using Application.BusinessModels.Shared.Triggers;
using Application.BusinessModels.Shippings.Actions;
using Application.BusinessModels.Shippings.Triggers;
using Application.BusinessModels.Tariffs.Triggers;
using Application.BusinessModels.Warehouses.Triggers;
using Application.Services;
using Application.Services.Addresses;
using Application.Services.AppConfiguration;
using Application.Services.Articles;
using Application.Services.BodyTypes;
using Application.Services.Clients;
using Application.Services.DeliveryTypes;
using Application.Services.Documents;
using Application.Services.DocumentTypes;
using Application.Services.FieldProperties;
using Application.Services.Files;
using Application.Services.History;
using Application.Services.Identity;
using Application.Services.Injections;
using Application.Services.Orders;
using Application.Services.PickingTypes;
using Application.Services.ProductTypes;
using Application.Services.Profile;
using Application.Services.Providers;
using Application.Services.Report;
using Application.Services.Roles;
using Application.Services.RoleTypes;
using Application.Services.Shippings;
using Application.Services.ShippingWarehouses;
using Application.Services.Tariffs;
using Application.Services.TaskProperties;
using Application.Services.Tonnages;
using Application.Services.Translations;
using Application.Services.TransportCompanies;
using Application.Services.Triggers;
using Application.Services.Users;
using Application.Services.UserSettings;
using Application.Services.VehicleTypes;
using Application.Services.WarehouseCity;
using Application.Services.Warehouses;
using Application.Shared;
using Application.Shared.FieldSetter;
using DAL;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Services.Articles;
using Domain.Services.BodyTypes;
using Domain.Services.Clients;
using Domain.Services.DeliveryTypes;
using Domain.Services.Documents;
using Domain.Services.DocumentTypes;
using Domain.Services.FieldProperties;
using Domain.Services.Files;
using Domain.Services.History;
using Domain.Services.Identity;
using Domain.Services.Injections;
using Domain.Services.Orders;
using Domain.Services.OrderTypes;
using Domain.Services.PickingTypes;
using Domain.Services.ProductTypes;
using Domain.Services.Profile;
using Domain.Services.Providers;
using Domain.Services.Report;
using Domain.Services.Roles;
using Domain.Services.Shippings;
using Domain.Services.ShippingWarehouses;
using Domain.Services.TarifficationTypes;
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
using OnChangeTarifficationType = Application.BusinessModels.Orders.Triggers.OnChangeTarifficationType;
using OnChangeVehicleTypeId = Application.BusinessModels.Orders.Triggers.OnChangeVehicleTypeId;

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
            services.AddScoped<IAuditDataService, AuditDataService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IDeliveryCostCalcService, DeliveryCostCalcService>();
            services.AddScoped<IShippingTarifficationTypeDeterminer, ShippingTarifficationTypeDeterminer>();
            services.AddScoped<IShippingCalculationService, ShippingCalculationService>();

            services.AddScoped<ITriggersService, TriggersService>();

            /*start of add service implementation*/
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddScoped<IShippingsService, ShippingsService>();
            services.AddScoped<ITariffsService, TariffsService>();
            services.AddScoped<IShippingWarehousesService, ShippingWarehousesService>();
            services.AddScoped<IShippingAddressService, ShippingAddressService>();
            services.AddScoped<IShippingGetRouteService, ShippingGetRouteService>();
            services.AddScoped<IDeliveryAddressService, DeliveryAddressService>();
            services.AddScoped<IWarehousesService, WarehousesService>();
            services.AddScoped<IShippingWarehousesForOrderCreation, ShippingWarehousesForOrderCreation>();
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
            services.AddScoped<IProductTypesService, ProductTypesService>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<IOrderShippingStatusService, OrderShippingStatusService>();
            services.AddScoped<IClientsService, ClientsService>();
            services.AddScoped<IProvidersService, ProvidersService>();

            services.AddScoped<ICleanAddressService, CleanAddressService>();
            services.AddScoped<IProfileService, ProfileService>();
            
            services.AddScoped<IRoleTypesService, RoleTypesService>();
            services.AddScoped<IOrderTypesService, OrderTypesService>();
            services.AddScoped<IDeliveryTypesService, DeliveryTypesService>();
            services.AddScoped<ITarifficationTypesService, TarifficationTypesService>();
            
                        
            services.AddScoped<IReportService, ReportService>();

            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IFieldSetterFactory, FieldSetterFactory>();
            services.AddScoped<IChangeTrackerFactory, ChangeTrackerFactory>();

            /*end of add service implementation*/

            AddOrderBusinessModels(services);
            AddShippingBusinessModels(services);
            AddDictionariesBusinessModels(services);

            InitDatabase(services, configuration, migrateDb);
        }

        private static void InitDatabase(IServiceCollection services, IConfiguration configuration, bool migrateDb)
        {
            var connectionString = configuration.GetConnectionString("DefaultDatabase");

            var buildServiceProvider = services.AddEntityFrameworkNpgsql()
                .AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(connectionString);
                })
                .BuildServiceProvider();

            var appDbContext = buildServiceProvider.GetService<AppDbContext>();

            if (migrateDb)
            {
                //appDbContext.DropDb();
                appDbContext.Migrate(connectionString);
            }

            var shippingsCount = appDbContext.Shippings.Count();
            ShippingNumberProvider.InitLastNumber(shippingsCount);
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
            services.AddScoped<IAppAction<Order>, RollbackOrder>();

            services.AddScoped<IGroupAppAction<Order>, UnionOrders>();
            services.AddScoped<IGroupAppAction<Order>, UnionOrdersInExisted>();

            services.AddScoped<IAppAction<Order>, SendOrderShippingToTk>();
            services.AddScoped<IAppAction<Order>, ConfirmOrderShipping>();
            services.AddScoped<IAppAction<Order>, RejectRequestOrderShipping>();
            services.AddScoped<IAppAction<Order>, CancelRequestOrderShipping>();
            services.AddScoped<IAppAction<Order>, CompleteOrderShipping>();
            services.AddScoped<IAppAction<Order>, CancelOrderShipping>();
            services.AddScoped<IAppAction<Order>, ProblemOrderShipping>();
            services.AddScoped<IAppAction<Order>, BillingOrderShipping>();
            services.AddScoped<IAppAction<Order>, ArchiveOrderShipping>();
            services.AddScoped<IAppAction<Order>, RollbackOrderShipping>();
            

            services.AddScoped<ITrigger<Order>, UpdateOrderDeliveryCost>();
            services.AddScoped<ITrigger<Order>, OnChangePalletsCountOrDeliveryRegion>();
            services.AddScoped<ITrigger<Order>, OnChangeTarifficationType>();
            services.AddScoped<ITrigger<Order>, OnChangeVehicleTypeId>();
            services.AddScoped<ITrigger<Order>, UpdateShippingRoute>();
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
            services.AddScoped<IAppAction<Shipping>, RollbackShipping>();
            services.AddScoped<IAppAction<Shipping>, SendToPooling>();

            services.AddScoped<ITrigger<Shipping>, UpdateShippingDeliveryCost>();
            services.AddScoped<ITrigger<Shipping>, Application.BusinessModels.Shippings.Triggers.OnChangeTarifficationType>();
            services.AddScoped<ITrigger<Shipping>, Application.BusinessModels.Shippings.Triggers.OnChangeVehicleTypeId>();
            services.AddScoped<ITrigger<Shipping>, OnChangeTransportCompany>();
        }

        private static void AddDictionariesBusinessModels(IServiceCollection services)
        {
            services.AddScoped<ITrigger<Tariff>, UpdateTariffDeliveryCost>();

            services.AddScoped<ITrigger<Warehouse>, ValidateDeliveryAddress>();
        }
    }
}

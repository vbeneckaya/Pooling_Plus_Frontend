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
using Application.Services.UserIdProvider;
using DAL;
using Domain.Services.AppConfiguration;
using Domain.Services.Identity;
using Domain.Services.Roles;
using Domain.Services.Translations;
using Domain.Services.UserIdProvider;
/*start of using domain service*/
using Domain.Services.Orders;
using Domain.Services.Shippings;
using Domain.Services.Tariffs;
using Domain.Services.Warehouses;
using Domain.Services.Articles;
using Domain.Services.TransportCompanies;
using Domain.Services.Users;
/*end of using domain service*/
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Installers
{
    public static class DomainInstaller
    {
        public static void AddDomain(this IServiceCollection services, IConfiguration configuration, bool migrateDb)
        {
            services.Add(new ServiceDescriptor(typeof(AppDbContext), typeof(AppDbContext), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IAppConfigurationService), typeof(AppConfigurationService), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IIdentityService), typeof(IdentityService), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IUserIdProvider), typeof(UserIdProvider), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IUsersService), typeof(UsersService), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IRolesService), typeof(RolesService), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(ITranslationsService), typeof(TranslationsService), ServiceLifetime.Scoped));

            /*start of add service implementation*/
            services.Add(new ServiceDescriptor(typeof(IOrdersService), typeof(OrdersService),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(IShippingsService), typeof(ShippingsService),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(ITariffsService), typeof(TariffsService),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(IWarehousesService), typeof(WarehousesService),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(IArticlesService), typeof(ArticlesService),  ServiceLifetime.Scoped) );
            services.Add(new ServiceDescriptor(typeof(ITransportCompaniesService), typeof(TransportCompaniesService),  ServiceLifetime.Scoped) );
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

                appDbContext.DropDb();
                appDbContext.SaveChanges();
                appDbContext.Migrate(connectionString);
            }
        }
    }
}

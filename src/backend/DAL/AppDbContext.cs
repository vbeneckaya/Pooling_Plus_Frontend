using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAL.Extensions;
using Domain.Persistables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.Extensions.Logging;
using ThinkingHome.Migrator;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
            
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<ICompositeMethodCallTranslator, CustomSqlMethodCallTranslator>();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Translation> Translations { get; set; }
        public DbSet<Injection> Injections { get; set; }
        public DbSet<TaskProperty> TaskProperties { get; set; }
        /*start of add DbSets*/
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<ShippingWarehouse> ShippingWarehouses { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<TransportCompany> TransportCompanies { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<PickingType> PickingTypes { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<FileStorage> FileStorage { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<HistoryEntry> HistoryEntries { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Tonnage> Tonnages { get; set; }
        public DbSet<BodyType> BodyTypes { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<FieldPropertyItem> FieldPropertyItems { get; set; }
        public DbSet<FieldPropertyItemVisibility> FieldPropertyVisibilityItems { get; set; }

        public DbSet<Company> Companies { get; set; }

        /*end of add DbSets*/

        public void Migrate(string connectionString)
        {
            
            using (var loggerFactory = new LoggerFactory())
            {
                var logger = loggerFactory.CreateLogger("Migration");

                using (var migrator = new Migrator("postgres", connectionString, Assembly.GetAssembly(typeof(AppDbContext)), logger))
                {
                  HashSet<long> applied = new HashSet<long>(migrator.GetAppliedMigrations());
                    foreach (var migrationInfo in migrator.AvailableMigrations.OrderBy(m => m.Version))
                    {
                        if (!applied.Contains(migrationInfo.Version))
                        {
                            migrator.ExecuteMigration(migrationInfo.Version, migrationInfo.Version - 1);
                        }
                    }
                }
            }
        }

        public void DropDb()
        {
            var commandText = "DROP SCHEMA public CASCADE;CREATE SCHEMA public;";
            Database.ExecuteSqlCommand(commandText);
        }
    }
}
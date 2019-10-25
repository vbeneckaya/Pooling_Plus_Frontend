using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domain.Persistables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ThinkingHome.Migrator;
using ThinkingHome.Migrator.Loader;

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
        /*end of add DbSets*/

        public void Migrate(string connectionString)
        {
            
            using (var loggerFactory = new LoggerFactory())
            {
                var logger = loggerFactory.CreateLogger("Migration");

                Assembly dalAssembly = Assembly.GetAssembly(typeof(AppDbContext));
                using (var migrator = new Migrator("postgres", connectionString, dalAssembly, logger))
                {
                    MigrationAssembly migrationAssembly = new MigrationAssembly(dalAssembly);
                    HashSet<long> applied = new HashSet<long>(migrator.GetAppliedMigrations());
                    foreach (var migrationInfo in migrator.AvailableMigrations.OrderBy(m => m.Version))
                    {
                        if (!applied.Contains(migrationInfo.Version))
                        {
                            var migration = migrationAssembly.InstantiateMigration(migrationInfo, migrator.Provider);
                            migration.Apply();
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
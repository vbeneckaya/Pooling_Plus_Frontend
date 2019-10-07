using System.Reflection;
using Domain.Persistables;
using Microsoft.EntityFrameworkCore;
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
        /*end of add DbSets*/

        public void Migrate(string connectionString)
        {
            
            using (var loggerFactory = new LoggerFactory())
            {
                var logger = loggerFactory.CreateLogger("Migration");

                using (var migrator = new Migrator("postgres", connectionString, Assembly.GetAssembly(typeof(AppDbContext)), logger))
                {
                    migrator.Migrate(-1);
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
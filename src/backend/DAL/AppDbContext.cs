﻿using System.Reflection;
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
        public DbSet<Transportation> Transportations { get; set; }
        public DbSet<Order> Orders { get; set; }
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
    }
}
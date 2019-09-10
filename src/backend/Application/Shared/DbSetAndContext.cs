using System;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Application.Shared
{
    public class DbSetAndContext<TEntity> : IDisposable where TEntity : class
    {
        public DbSet<TEntity> DbSet { get; set; }
        public AppDbContext Context { get; set; }
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
using Domain.Persistables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace DAL.Services
{
    public class CommonDataService : ICommonDataService
    {
        private readonly AppDbContext _context;

        public CommonDataService(AppDbContext context)
        {
            _context = context;
        }

        public TEntity GetById<TEntity>(Guid id) where TEntity : class, IPersistable
        {
            return GetDbSet<TEntity>().Find(id);
        }

        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class, IPersistable
        {
            return _context.Set<TEntity>();
        }

        public EntityEntry<TEntity> GetTrackingEntry<TEntity>(TEntity entity) where TEntity : class, IPersistable
        {
            return _context.Entry<TEntity>(entity);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}

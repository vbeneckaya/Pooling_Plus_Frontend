using Domain.Persistables;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<EntityChanges<TEntity>> GetChanges<TEntity>() where TEntity : class, IPersistable
        {
            var entries = _context.ChangeTracker.Entries<TEntity>().ToList();
            foreach (var entry in entries)
            {
                var fieldChanges = GetFieldChanges(entry);

                yield return new EntityChanges<TEntity>
                {
                    Entity = entry.Entity,
                    FieldChanges = fieldChanges
                };
            }
        }

        public IEnumerable<EntityChanges> GetChanges()
        {
            var entries = _context.ChangeTracker.Entries().ToList();
            foreach (var entry in entries)
            {
                var fieldChanges = GetFieldChanges(entry);

                yield return new EntityChanges
                {
                    Entity = (IPersistable)entry.Entity,
                    FieldChanges = fieldChanges
                };
            }
        }

        private List<EntityFieldChanges> GetFieldChanges(EntityEntry entity)
        {
            var fieldChanges = new List<EntityFieldChanges>();
            foreach (var field in entity.Properties.Where(x => x.IsModified || entity.State == EntityState.Added).ToList())
            {
                var fieldChange = new EntityFieldChanges
                {
                    FieldName = field.Metadata.Name,
                    OldValue = field.OriginalValue,
                    NewValue = field.CurrentValue
                };
                fieldChanges.Add(fieldChange);
            }
            return fieldChanges;
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }

        void ICommonDataService.Remove<TEntity>(TEntity entity)
        {
            this.GetDbSet<TEntity>().Remove(entity);
        }
    }
}

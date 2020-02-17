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

        public TEntity GetByIdOrNull<TEntity>(Guid? id) where TEntity : class, IPersistable
        {
            return id != null ? GetById<TEntity>(id.Value) : null;
        }

        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class, IPersistable
        {
            return _context.Set<TEntity>();
        }

        public Guid? CreateIfNotExisted<TEntity>(string fieldName, string value) where TEntity : class, IPersistable
        {
            var eType = typeof(TEntity);
            var entity = eType.Assembly.CreateInstance(eType.Name);
            if (entity == null) return null;
            
            var newId = Guid.NewGuid();
            var propId = eType.GetProperty("Id");
            propId.SetValue(entity, newId);
                
            var propName = eType.GetProperty(fieldName);
            propName.SetValue(entity, value);
            _context.SaveChanges();
            return newId;
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
            var fields = entity.Properties.Where(x => x.IsModified || (entity.State == EntityState.Added && x.CurrentValue != default))
                                          .ToList();
            foreach (var field in fields)
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

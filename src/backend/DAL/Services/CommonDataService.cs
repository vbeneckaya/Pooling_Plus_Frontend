using Domain.Persistables;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<EntityChangesDto<TEntity>> GetChanges<TEntity>() where TEntity : class, IPersistable
        {
            var entries = _context.ChangeTracker.Entries<TEntity>().ToList();
            foreach (var entry in entries)
            {
                var fieldChanges = new List<EntityFieldChangesDto>();
                foreach (var field in entry.Properties.Where(x => x.IsModified).ToList())
                {
                    var fieldChange = new EntityFieldChangesDto
                    {
                        FieldName = field.Metadata.Name,
                        OldValue = field.OriginalValue,
                        NewValue = field.CurrentValue
                    };
                    fieldChanges.Add(fieldChange);
                }

                yield return new EntityChangesDto<TEntity>
                {
                    Entity = entry.Entity,
                    FieldChanges = fieldChanges
                };
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}

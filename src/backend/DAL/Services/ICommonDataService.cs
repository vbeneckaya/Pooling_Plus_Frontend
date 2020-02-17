using Domain.Persistables;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DAL.Services
{
    public interface ICommonDataService
    {
        DbSet<TEntity> GetDbSet<TEntity>() where TEntity: class, IPersistable;

        TEntity GetById<TEntity>(Guid id) where TEntity : class, IPersistable;
        
        TEntity GetByIdOrNull<TEntity>(Guid? id) where TEntity : class, IPersistable;

        IEnumerable<EntityChanges<TEntity>> GetChanges<TEntity>() where TEntity : class, IPersistable;

        void Remove<TEntity>(TEntity entity) where TEntity : class, IPersistable;

        void SaveChanges();
    }
}

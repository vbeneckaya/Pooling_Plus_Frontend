using Domain.Persistables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace DAL.Services
{
    public interface ICommonDataService
    {
        DbSet<TEntity> GetDbSet<TEntity>() where TEntity: class, IPersistable;

        TEntity GetById<TEntity>(Guid id) where TEntity : class, IPersistable;

        EntityEntry<TEntity> GetTrackingEntry<TEntity>(TEntity entity) where TEntity : class, IPersistable;

        void SaveChanges();
    }
}

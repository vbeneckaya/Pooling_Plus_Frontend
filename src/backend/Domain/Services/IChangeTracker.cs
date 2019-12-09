using Domain.Persistables;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Services
{
    public interface IChangeTracker
    {
        IChangeTracker Add<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property);

        IChangeTracker Remove<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property);

        IChangeTracker TrackAll<TEntity>();

        void LogTrackedChanges<TEntity>() where TEntity : class, IPersistable;
    }
}

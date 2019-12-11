using Domain.Persistables;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Application.BusinessModels.Shared.Handlers
{
    public interface IFieldSetter<TEntity> where TEntity : class, IPersistable
    {
        IFieldSetter<TEntity> AddHandler<TProperty>(Expression<Func<TEntity, TProperty>> property, IFieldHandler<TEntity, TProperty> handler);

        void Appy(EntityChanges<TEntity> changes);
    }
}

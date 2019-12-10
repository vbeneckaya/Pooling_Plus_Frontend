using Application.BusinessModels.Shared.Handlers;
using Domain.Persistables;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Application.Shared.FieldSetter
{
    public class FieldUpdater<TEntity> : IFieldSetter<TEntity>
        where TEntity : class, IPersistable
    {
        private readonly Dictionary<string, Action<IPersistable, EntityFieldChanges>> _afterActions = new Dictionary<string, Action<IPersistable, EntityFieldChanges>>();

        public IFieldSetter<TEntity> AddHandler<TProperty>(Expression<Func<TEntity, TProperty>> property, IFieldHandler<TEntity, TProperty> handler)
        {
            _afterActions.Add(GetPropertyName(property), (IPersistable entity, EntityFieldChanges change) =>
            {
                handler.AfterChange((TEntity)entity, (TProperty)change.OldValue, (TProperty)change.NewValue);
            });

            return this;
        }

        public void Appy(EntityChanges<TEntity> changes)
        {
            foreach (var change in changes.FieldChanges)
            {
                if (this._afterActions.ContainsKey(change.FieldName))
                {
                    this._afterActions[change.FieldName](changes.Entity, change);
                }
            }
        }

        private string GetPropertyName<TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            var propertyBody = property.Body as MemberExpression;

            if (propertyBody == null) return null;

            var propertyInfo = propertyBody.Member as PropertyInfo;

            if (propertyInfo == null) return null;

            return  propertyInfo.Name;
        }
    }
}

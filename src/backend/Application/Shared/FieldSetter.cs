using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Shared
{
    public class FieldSetter<TEntity>
    {
        public bool UpdateField<T>(Expression<Func<TEntity, T>> property, T newValue, params Action<TEntity, T, T>[] afterActions)
        {
            T oldValue = property.Compile()(_entity);
            if (!Equals(oldValue, newValue))
            {
                var propertyBody = property.Body as MemberExpression;
                if (propertyBody != null)
                {
                    var propertyInfo = propertyBody.Member as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(_entity, newValue);
                        if (afterActions != null)
                        {
                            foreach (Action<TEntity, T, T> action in afterActions)
                            {
                                _afterActions.Add(() => action(_entity, oldValue, newValue));
                            }
                        }
                        HasChanges = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public void ApplyAfterActions()
        {
            foreach (var action in _afterActions)
            {
                action();
            }
        }

        public bool HasChanges { get; private set; } = false;

        public FieldSetter(TEntity entity)
        {
            _entity = entity;
        }

        private readonly TEntity _entity;
        private readonly List<Action> _afterActions = new List<Action>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Shared
{
    public class FieldSetter<TEntity>
    {
        public bool UpdateField<T>(Expression<Func<TEntity, T>> property, T newValue, params Action<TEntity>[] afterActions)
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
                            _afterActions.AddRange(afterActions);
                        }
                        _hasChanges = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public void ApplyAfterActions()
        {
            foreach (var action in _afterActions.Distinct())
            {
                action(_entity);
            }
        }

        public bool HasChanges => _hasChanges;

        public FieldSetter(TEntity entity)
        {
            _entity = entity;
        }

        private bool _hasChanges = false;
        private readonly TEntity _entity;
        private readonly List<Action<TEntity>> _afterActions = new List<Action<TEntity>>();
    }
}

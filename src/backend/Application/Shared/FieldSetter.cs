using Application.BusinessModels.Shared.Handlers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Shared
{
    public class FieldSetter<TEntity>
    {
        public bool UpdateField<T>(Expression<Func<TEntity, T>> property, T newValue, IFieldHandler<TEntity, T> fieldHandler = null)
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
                        if (fieldHandler != null)
                        {
                            string error = fieldHandler.ValidateChange(_entity, oldValue, newValue);
                            if (!string.IsNullOrEmpty(error))
                            {
                                _validationErrors.Add(error);
                                return false;
                            }
                        }

                        propertyInfo.SetValue(_entity, newValue);
                        if (fieldHandler != null)
                        {
                            _afterActions.Add(() => fieldHandler.AfterChange(_entity, oldValue, newValue));
                        }
                        foreach (var action in _commonActions)
                        {
                            _afterActions.Add(() => action(_entity, propertyInfo.Name, newValue));
                        }
                        HasChanges = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public void AddCommonAction(Action<TEntity, string, object> action)
        {
            _commonActions.Add(action);
        }

        public void ApplyAfterActions()
        {
            foreach (var action in _afterActions)
            {
                action();
            }
        }

        public bool HasChanges { get; private set; } = false;

        public string ValidationErrors => string.Join(". ", _validationErrors);

        public FieldSetter(TEntity entity)
        {
            _entity = entity;
        }

        private readonly TEntity _entity;
        private readonly List<Action<TEntity, string, object>> _commonActions = new List<Action<TEntity, string, object>>();
        private readonly List<Action> _afterActions = new List<Action>();
        private readonly List<string> _validationErrors = new List<string>();
    }
}

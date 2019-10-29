using Application.BusinessModels.Shared.Handlers;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Shared
{
    public class FieldSetter<TEntity> where TEntity : IPersistable
    {
        public bool UpdateField<T>(
            Expression<Func<TEntity, T>> property, 
            T newValue, 
            IFieldHandler<TEntity, T> fieldHandler = null, 
            bool ignoreChanges = false,
            Func<T, string> nameLoader = null)
        {
            T oldValue = property.Compile()(Entity);
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
                            string error = fieldHandler.ValidateChange(Entity, oldValue, newValue);
                            if (!string.IsNullOrEmpty(error))
                            {
                                _validationErrors.Add(error);
                                return false;
                            }
                        }

                        propertyInfo.SetValue(Entity, newValue);

                        if (!ignoreChanges)
                        {
                            if (fieldHandler != null)
                            {
                                _afterActions.Add(() => fieldHandler.AfterChange(Entity, oldValue, newValue));
                            }

                            string fieldName = propertyInfo.Name;
                            object historyOldValue = nameLoader == null || oldValue == null ? (object)oldValue : nameLoader(oldValue);
                            object historyNewValue = nameLoader == null || newValue == null ? (object)newValue : nameLoader(newValue);
                            _historyActions[fieldName] = () => SaveHistory(fieldName, historyOldValue, historyNewValue);
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

        public void SaveHistoryLog()
        {
            foreach (var action in _historyActions.Values)
            {
                action();
            }
        }

        public TEntity Entity { get; }

        public bool HasChanges { get; private set; } = false;

        public string ValidationErrors => string.Join(". ", _validationErrors);

        private void SaveHistory(string fieldName, object oldValue, object newValue)
        {
            _historyService.Save(Entity.Id, "fieldChanged", fieldName?.ToLowerFirstLetter(), oldValue, newValue);
        }

        public FieldSetter(TEntity entity, IHistoryService historyService)
        {
            Entity = entity;
            _historyService = historyService;
        }

        private readonly IHistoryService _historyService;
        private readonly List<Action> _afterActions = new List<Action>();
        private readonly Dictionary<string, Action> _historyActions = new Dictionary<string, Action>();
        private readonly List<string> _validationErrors = new List<string>();
    }
}

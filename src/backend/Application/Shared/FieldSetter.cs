using Application.BusinessModels.Shared.Handlers;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Collections.Generic;
using System.Linq;
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

            if (Equals(oldValue, newValue)) return false;
            
            var propertyBody = property.Body as MemberExpression;

            if (propertyBody == null) return false;
           
            var propertyInfo = propertyBody.Member as PropertyInfo;

            if (propertyInfo == null) return false;

            string modelFieldName = propertyInfo.Name.ToLowerFirstLetter();
            //if (_readOnlyFields != null && _readOnlyFields.Contains(modelFieldName))
            //{
            //    _validationErrors.Add($"{propertyInfo.Name} is Read Only");
            //    return false;
            //}

            //if (fieldHandler != null)
            //{
            //    string error = fieldHandler.ValidateChange(Entity, oldValue, newValue);
            //    if (!string.IsNullOrEmpty(error))
            //    {
            //        _validationErrors.Add(error);
            //        return false;
            //    }
            //}

            propertyInfo.SetValue(Entity, newValue);

            if (fieldHandler != null)
            {
                _afterActions.Add(() => fieldHandler.AfterChange(Entity, oldValue, newValue));
            }

            HasChanges = true;
            return true;
        }

        public void ApplyAfterActions()
        {
            foreach (var action in _afterActions)
            {
                action();
            }
        }

        public TEntity Entity { get; }

        public bool HasChanges { get; private set; } = false;

        //public string ValidationErrors => string.Join(". ", _validationErrors);

        public FieldSetter(TEntity entity, IEnumerable<string> readOnlyFields = null)
        {
            Entity = entity;
            _readOnlyFields = readOnlyFields?.ToList();
        }

        private readonly List<string> _readOnlyFields;
        private readonly List<Action> _afterActions = new List<Action>();
        private readonly Dictionary<string, Action> _historyActions = new Dictionary<string, Action>();
        //private readonly List<string> _validationErrors = new List<string>();
    }
}

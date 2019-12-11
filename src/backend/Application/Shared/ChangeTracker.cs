using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Shared
{
    public class ChangeTracker : IChangeTracker
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        Dictionary<string, EntityTrackerConfiguration> TypeConfigurations { get; set; } = new Dictionary<string, EntityTrackerConfiguration>();

        public ChangeTracker(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }
        public IChangeTracker Add<TEntity>(Expression<Func<TEntity, object>> property)
        {
            var typeName = typeof(TEntity).Name;
            var prop = this.GetProperty(property);

            Add(typeName, prop);

            return this;
        }

        private void Add(string typeName, PropertyInfo property)
        { 
            var config = GetTypeConfiguration(typeName);
            config.Properties.Add(property);
        }

        public IChangeTracker Remove<TEntity>(Expression<Func<TEntity, object>> property)
        {
            var typeName = typeof(TEntity).Name;

            var config = GetTypeConfiguration(typeName);

            var prop = this.GetProperty(property);

            config.Properties.Remove(prop);

            return this;
        }

        public IChangeTracker TrackAll<TEntity>()
        {
            var properties = typeof(TEntity).GetProperties();
            var typeName = typeof(TEntity).Name;

            foreach (var prop in properties)
            {
                if (!prop.GetCustomAttributes<IgnoreHistoryAttribute>().Any())
                {
                    Add(typeName, prop);
                }
            }

            return this;
        }

        public void LogTrackedChanges<TEntity>(EntityChanges<TEntity> change) where TEntity : class, IPersistable
        {
            if (change?.FieldChanges == null) return;

            var config = GetTypeConfiguration(typeof(TEntity).Name);

            var changes = change.FieldChanges.Where(f => config.Properties.Any(x => x.Name == f.FieldName)).ToList();

            foreach (var field in changes)
            {
                var property = config.Properties.FirstOrDefault(x => x.Name == field.FieldName);

                object newValue = field.NewValue;
                if (newValue != null && (property?.PropertyType == typeof(Guid) || property?.PropertyType == typeof(Guid?)))
                {
                    newValue = LoadReferenceName(field, property) ?? newValue;
                }

                _historyService.Save(change.Entity.Id, "fieldChanged",
                                        field.FieldName.ToLowerFirstLetter(),
                                        field.OldValue, newValue);
            }
        }

        private EntityTrackerConfiguration GetTypeConfiguration(string typeName)
        {
            if (!TypeConfigurations.ContainsKey(typeName))
            {
                TypeConfigurations.Add(typeName, new EntityTrackerConfiguration { TypeName = typeName });
            }

            return TypeConfigurations[typeName];
        }

        private Type GetReferenceType(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<ReferenceTypeAttribute>();
            return attr?.Type;
        }

        private object LoadReferenceName(EntityFieldChanges field, PropertyInfo property)
        {
            Type refType = GetReferenceType(property);
            if (refType != null)
            {
                object refId = field.NewValue;
                if (property.PropertyType == typeof(Guid?))
                {
                    refId = ((Guid?)refId).Value;
                }
                var getMethod = _dataService.GetType().GetMethod(nameof(_dataService.GetById)).MakeGenericMethod(refType);
                var refEntity = getMethod.Invoke(_dataService, new[] { refId });
                return refEntity?.ToString();
            }
            return null;
        }

        private PropertyInfo GetProperty<TEntity, T>(Expression<Func<TEntity, T>> property)
        {
            var propertyBody = property.Body as MemberExpression;

            if (propertyBody == null) return null;

            var propertyInfo = propertyBody.Member as PropertyInfo;

            return propertyInfo;
        }
        private class EntityTrackerConfiguration
        { 
            public string TypeName { get; set; }

            public List<PropertyInfo> Properties { get; set; } = new List<PropertyInfo>();
        }
    }
}

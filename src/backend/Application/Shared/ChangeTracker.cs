using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.History;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

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
            var propName = this.GetPropertyName(property);

            Add(typeName, propName);

            return this;
        }

        private void Add(string typeName, string propertyName)
        { 
            var config = GetTypeConfiguration(typeName);
            config.Properties.Add(propertyName);
        }

        public IChangeTracker Remove<TEntity>(Expression<Func<TEntity, object>> property)
        {
            var typeName = typeof(TEntity).Name;

            var config = GetTypeConfiguration(typeName);

            var propName = this.GetPropertyName(property);

            config.Properties.Remove(propName);

            return this;
        }

        public IChangeTracker TrackAll<TEntity>()
        {
            var properties = typeof(TEntity).GetProperties();
            var typeName = typeof(TEntity).Name;

            foreach (var prop in properties)
            {
                Add(typeName, prop.Name);
            }

            return this;
        }


        public IEnumerable<EntityChanges<TEntity>> GetTrackedChanges<TEntity>() where TEntity : class, IPersistable
        {
            var config = TypeConfigurations[typeof(TEntity).Name];

            var changes = _dataService.GetChanges<TEntity>();

            return changes.Select(i => new EntityChanges<TEntity>
            { 
                Entity = i.Entity,
                FieldChanges = i.FieldChanges.Where(f => config.Properties.Contains(f.FieldName)).ToList()
            });
        }

        public void LogTrackedChanges<TEntity>(EntityChanges<TEntity> change) where TEntity : class, IPersistable
        {
            var config = GetTypeConfiguration(typeof(TEntity).Name);

            var changes = change.FieldChanges.Where(f => config.Properties.Contains(f.FieldName)).ToList();

            foreach (var field in changes)
            {
                _historyService.Save(change.Entity.Id, "fieldChanged",
                                        field.FieldName.ToLowerFirstLetter(),
                                        field.OldValue, field.NewValue);
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

        private string GetPropertyName<TEntity, T>(Expression<Func<TEntity, T>> property)
        {
            var propertyBody = property.Body as MemberExpression;

            if (propertyBody == null) return null;

            var propertyInfo = propertyBody.Member as PropertyInfo;

            if (propertyInfo == null) return null;

            return propertyInfo.Name;
        }
        private class EntityTrackerConfiguration
        { 
            public string TypeName { get; set; }

            public List<string> Properties { get; set; } = new List<string>();
        }
    }
}

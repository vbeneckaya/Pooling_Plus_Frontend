using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Application.Shared;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Shared.BulkUpdates
{
    public class BaseBulkUpdate<TEntity> : IBulkUpdate<TEntity> where TEntity : IPersistable
    {
        private readonly IHistoryService _historyService;

        public BaseBulkUpdate(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        public string FieldName => null;

        public FieldType FieldType => FieldType.Text;

        public bool IsAvailable(Role role, TEntity target)
        {
            return true;
        }

        public AppActionResult Update(CurrentUserDto user, TEntity target, string fieldName, string value)
        {
            var propertyInfo = typeof(TEntity).GetProperties().Where(x => string.Compare(x.Name, fieldName, true) == 0).FirstOrDefault();
            if (propertyInfo == null)
            {
                return new AppActionResult
                {
                    IsError = true,
                    Message = "Not found"
                };
            }

            var setter = new FieldSetter<TEntity>(target, _historyService);

            if (propertyInfo.PropertyType == typeof(decimal?))
            {
                decimal? validValue = ParseDecimal(value);
                Set(setter, propertyInfo.Name, validValue);
            }
            else if (propertyInfo.PropertyType == typeof(int?))
            {
                int? validValue = ParseInt(value);
                Set(setter, propertyInfo.Name, validValue);
            }
            else if (propertyInfo.PropertyType == typeof(TimeSpan?))
            {
                TimeSpan? validValue = ParseTimeSpan(value);
                Set(setter, propertyInfo.Name, validValue);
            }
            else if (propertyInfo.PropertyType == typeof(DateTime?))
            {
                DateTime? validValue = ParseDateTime(value);
                Set(setter, propertyInfo.Name, validValue);
            }
            else if (propertyInfo.PropertyType == typeof(Guid?))
            {
                Guid? validValue = ParseGuid(value);
                Set(setter, propertyInfo.Name, validValue);
            }
            else if (propertyInfo.PropertyType.IsEnum)
            {
                var parseMethodInfo = GetType().GetMethod("SetEnum");
                var parseMethodRef = parseMethodInfo.MakeGenericMethod(propertyInfo.PropertyType);
                var validValue = parseMethodRef.Invoke(this, new object[] { setter, propertyInfo.Name, value });
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                Set(setter, propertyInfo.Name, value);
            }

            string errors = setter.ValidationErrors;
            bool hasErrors = !string.IsNullOrEmpty(errors);
            if (!hasErrors)
            {
                setter.SaveHistoryLog();
            }

            return new AppActionResult
            {
                IsError = hasErrors,
                Message = errors
            };
        }

        private void Set<TValue>(FieldSetter<TEntity> setter, string fieldName, TValue value)
        {
            ParameterExpression param = Expression.Parameter(typeof(TEntity), string.Empty);
            MemberExpression prop = Expression.PropertyOrField(param, fieldName);
            var propertyLambda = Expression.Lambda<Func<TEntity, TValue>>(prop, param);
            setter.UpdateField(propertyLambda, value);
        }

        private decimal? ParseDecimal(string value)
        {
            if (decimal.TryParse((value ?? string.Empty).Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal decValue))
            {
                return decValue;
            }
            return null;
        }

        private int? ParseInt(string value)
        {
            if (int.TryParse(value ?? string.Empty, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
            {
                return intValue;
            }
            return null;
        }

        private Guid? ParseGuid(string value)
        {
            if (Guid.TryParse(value ?? string.Empty, out Guid idValue))
            {
                return idValue;
            }
            return null;
        }

        private DateTime? ParseDateTime(string value)
        {
            if ((value ?? string.Empty).TryParseDateTime(out DateTime dtValue))
            {
                return dtValue;
            }
            return null;
        }

        private TimeSpan? ParseTimeSpan(string value)
        {
            if (TimeSpan.TryParse(value ?? string.Empty, CultureInfo.InvariantCulture, out TimeSpan tsValue))
            {
                return tsValue;
            }
            return null;
        }

        private void SetEnum<TEnum>(FieldSetter<TEntity> setter, string fieldName, string value) where TEnum : Enum
        {
            if (Enum.TryParse(typeof(TEnum), value, true, out object enumValue))
            {
                Set(setter, fieldName, (TEnum)enumValue);
            }
        }
    }
}

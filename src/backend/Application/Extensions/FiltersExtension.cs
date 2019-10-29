using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Application.Extensions
{
    /// <summary>
    /// Filter Extentions
    /// </summary>
    public static class FiltersExtentions
    {
        /// <summary>
        /// Apply numeric filter (float)
        /// </summary>
        public static string ApplyNumericFilter<TModel>(this string search, Expression<Func<TModel, decimal?>> property, ref List<object> parameters)
        {
            if (string.IsNullOrEmpty(search)) return string.Empty;

            string fieldName = property.GetPropertyName();
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;

            decimal searchValue;
            if (!decimal.TryParse(search.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out searchValue)) return string.Empty;

            int paramInd = parameters.Count();
            parameters.Add(searchValue);
            return $@"ROUND(""{fieldName}"",3) = ROUND({{{paramInd}}},3)";
        }

        public static string ApplyBoolenFilter<TModel>(this string search, Expression<Func<TModel, bool?>> property, ref List<object> parameters)
        {
            if (string.IsNullOrEmpty(search)) return string.Empty;

            string fieldName = property.GetPropertyName();
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;

            bool searchValue;
            if (!bool.TryParse(search, out searchValue)) return string.Empty;

            int paramInd = parameters.Count();
            parameters.Add(searchValue);
            return $@"""{fieldName}"" = {{{paramInd}}}";
        }

        /// <summary>
        /// Apply numeric filter (integer)
        /// </summary>
        public static string ApplyNumericFilter<TModel>(this string search, Expression<Func<TModel, int?>> property, ref List<object> parameters)
        {
            if (string.IsNullOrEmpty(search)) return string.Empty;

            string fieldName = property.GetPropertyName();
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;

            int searchValue;
            if (!int.TryParse(search, NumberStyles.Integer, CultureInfo.InvariantCulture, out searchValue)) return string.Empty;

            int paramInd = parameters.Count();
            parameters.Add(searchValue);
            return $@"""{fieldName}"" = {{{paramInd}}}";
        }

        /// <summary>
        /// Apply boolean filter
        /// </summary>
        public static string ApplyBooleanFilter<TModel>(this string search, Expression<Func<TModel, bool>> property, ref List<object> parameters)
        {
            if (string.IsNullOrEmpty(search)) return string.Empty;

            string fieldName = property.GetPropertyName();
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;

            bool searchValue = search.ToLower() == "true";

            int paramInd = parameters.Count();
            parameters.Add(searchValue);
            return $@"""{fieldName}"" = {{{paramInd}}}";
        }

        /// <summary>
        /// Apply date range filter
        /// </summary>
        public static string ApplyDateRangeFilter<TModel>(this string search, Expression<Func<TModel, DateTime?>> property, ref List<object> parameters)
        {
            if (string.IsNullOrEmpty(search)) return string.Empty;

            string fieldName = property.GetPropertyName();
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;

            var dates = search.Split("-");

            var fromDateStr = dates.FirstOrDefault();
            var toDateStr = dates.ElementAtOrDefault(1);

            StringBuilder result = new StringBuilder();

            if (TryExtractDateTime(fromDateStr, out DateTime fromDate))
            {
                int paramInd = parameters.Count();
                parameters.Add(fromDate);
                result.Append($@"""{fieldName}"" >= {{{paramInd}}}");
            }

            if (TryExtractDateTime(toDateStr, out DateTime toDate))
            {
                if (result.Length > 0)
                {
                    result.Append(" AND ");
                }
                toDate = toDate.AddDays(1);
                int paramInd = parameters.Count();
                parameters.Add(toDate);
                result.Append($@"""{fieldName}"" < {{{paramInd}}}");
            }

            return result.ToString();
        }

        /// <summary>
        /// Apply time range filter
        /// </summary>
        public static string ApplyTimeRangeFilter<TModel>(this string search, Expression<Func<TModel, TimeSpan?>> property, ref List<object> parameters)
        {
            if (string.IsNullOrEmpty(search)) return string.Empty;

            string fieldName = property.GetPropertyName();
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;

            string field = $@"""{fieldName}""";
            return search.ApplyTimeRangeFilterBase<TModel>(field, ref parameters);
        }

        /// <summary>
        /// Apply time range filter
        /// </summary>
        public static string ApplyTimeRangeFilter<TModel>(this string search, Expression<Func<TModel, DateTime?>> property, ref List<object> parameters)
        {
            if (string.IsNullOrEmpty(search)) return string.Empty;

            string fieldName = property.GetPropertyName();
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;

            string field = $@"to_char(""{fieldName}"", 'HH24:MI:SS')::time";
            return search.ApplyTimeRangeFilterBase<TModel>(field, ref parameters);
        }

        /// <summary>
        /// Apply string filter
        /// </summary>
        public static string ApplyStringFilter<TModel>(this string search, Expression<Func<TModel, string>> property, ref List<object> parameters)
        {
            if (string.IsNullOrEmpty(search)) return string.Empty;

            string fieldName = property.GetPropertyName();
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;

            int paramInd = parameters.Count();
            parameters.Add($"%{search}%");
            return $@"""{fieldName}"" ~~* {{{paramInd}}}";
        }

        /// <summary>
        /// Apply options filter
        /// </summary>
        public static string ApplyOptionsFilter<TModel, TValue>(this string search, Expression<Func<TModel, TValue>> property, ref List<object> parameters)
        {
            return search.ApplyOptionsFilterBase(property, ref parameters, x => x);
        }

        /// <summary>
        /// Apply options filter
        /// </summary>
        public static string ApplyOptionsFilter<TModel, TValue>(this string search, Expression<Func<TModel, TValue>> property, ref List<object> parameters, Func<string, object> selection)
        {
            return search.ApplyOptionsFilterBase(property, ref parameters, selection);
        }

        /// <summary>
        /// Apply options filter
        /// </summary>
        public static string ApplyEnumFilter<TModel, TEnum>(this string search, Expression<Func<TModel, TEnum>> property, ref List<object> parameters) 
            where TEnum : struct
        {
            return search.ApplyOptionsFilterBase(property, ref parameters, x => MapFromStateDto<TEnum>(x));
        }

        /// <summary>
        /// Apply options filter
        /// </summary>
        public static string ApplyEnumFilter<TModel, TEnum>(this string search, Expression<Func<TModel, TEnum?>> property, ref List<object> parameters)
            where TEnum : struct
        {
            return search.ApplyOptionsFilterBase(property, ref parameters, x => MapFromStateDto<TEnum>(x));
        }

        /// <summary>
        /// Add WHERE condition with AND operator
        /// </summary>
        public static string WhereAnd(this string where, string condition)
        {
            if (string.IsNullOrEmpty(condition))
            {
                return where;
            }
            else if (string.IsNullOrEmpty(where))
            {
                return $"WHERE {condition}";
            }
            else
            {
                return $"{where} AND {condition}";
            }
        }

        public static IQueryable<TModel> OrderBy<TModel>(this IQueryable<TModel> query, string propertyName, bool? descending)
        {
            if (string.IsNullOrEmpty(propertyName)) return query;

            var propertyInfo = typeof(TModel).GetProperties()
                .FirstOrDefault(i => i.Name.ToLower() == propertyName.ToLower());

            if (propertyInfo == null) return query;

            ParameterExpression param = Expression.Parameter(typeof(TModel), string.Empty);
            MemberExpression property = Expression.PropertyOrField(param, propertyInfo.Name);
            LambdaExpression sort = Expression.Lambda(property, param);
            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                 "OrderBy" + (descending.GetValueOrDefault() ? "Descending" : string.Empty),
                new[] { typeof(TModel), property.Type },
                query.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<TModel>)query.Provider.CreateQuery<TModel>(call);
        }

        public static IQueryable<TSource> DefaultOrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool secondarySort)
        {
            IOrderedQueryable<TSource> ordered = source as IOrderedQueryable<TSource>;

            if (secondarySort && ordered != null)
            {
                return ordered.ThenByDescending(keySelector);
            }
            else
            {
                return source.OrderByDescending(keySelector);
            }
        }

        private static TEnum MapFromStateDto<TEnum>(string dtoStatus) where TEnum : struct
        {
            var mapFromStateDto = Enum.Parse<TEnum>(dtoStatus.ToUpperFirstLetter());

            return mapFromStateDto;
        }

        private static string GetPropertyName<TModel, TValue>(this Expression<Func<TModel, TValue>> property)
        {
            var propertyBody = property?.Body as MemberExpression;
            if (propertyBody != null)
            {
                var propertyInfo = propertyBody.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    return propertyInfo.Name;
                }
            }
            return null;
        }

        public static string ApplyOptionsFilterBase<TModel, TValue>(this string search,
            Expression<Func<TModel, TValue>> property,
            ref List<object> parameters,
            Func<string, object> parameterValueLookup)
        {
            if (string.IsNullOrEmpty(search)) return string.Empty;

            string fieldName = property.GetPropertyName();
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;

            var values = search.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!values.Any()) return string.Empty;

            StringBuilder inValue = new StringBuilder();
            foreach (string value in values)
            {
                if (inValue.Length > 0)
                {
                    inValue.Append(',');
                }
                int paramInd = parameters.Count();
                parameters.Add(parameterValueLookup(value));
                inValue.Append($"{{{paramInd}}}");
            }

            return $@"""{fieldName}"" in ({inValue})";
        }

        private static string ApplyTimeRangeFilterBase<TModel>(this string search, string field, ref List<object> parameters)
        {
            var times = search.Split("-");

            var fromTimeStr = times.FirstOrDefault();
            var toTimeStr = times.ElementAtOrDefault(1);

            StringBuilder result = new StringBuilder();

            if (TimeSpan.TryParse(fromTimeStr, out TimeSpan fromTime))
            {
                int paramInd = parameters.Count();
                parameters.Add(fromTime);
                result.Append($@"{field} >= {{{paramInd}}}");
            }

            if (TimeSpan.TryParse(toTimeStr, out TimeSpan toTime))
            {
                if (result.Length > 0)
                {
                    result.Append(" AND ");
                }
                int paramInd = parameters.Count();
                parameters.Add(toTime);
                result.Append($@"{field} <= {{{paramInd}}}");
            }

            return result.ToString();
        }

        private static bool TryExtractDateTime(string value, out DateTime result)
        {
            if (!DateTime.TryParseExact(
                value, 
                new[] {
                    "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm", "dd.MM.yyyy HH:mm",
                    "MM/dd/yyyy HH:mm:ss", "MM/dd/yyyy HH:mm", "MM/dd/yyyy HH:mm",
                    "dd.MM.yyyy", "MM/dd/yyyy"
                }, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.None, 
                out result))
            {
                if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

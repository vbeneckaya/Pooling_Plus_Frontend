﻿using Domain.Extensions;
using Domain.Shared;
using Domain.Shared.FormFilters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Application.Extensions
{
    /// <summary>
    /// Filter Extentions
    /// </summary>
    public static class FiltersExtentions
    {
        /// <summary>
        /// Apply numeric filter
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="filterData"></param>
        /// <returns></returns>
        public static IQueryable<TModel> ApplyNumericFilter<TModel>(this IQueryable<TModel> query, Expression<Func<TModel, int>> property, int? filterData)
        {
            if (!filterData.HasValue) return query;

            var propertyExpression = GetPropertyEqualExpression(property, filterData.Value);

            return query.Where(propertyExpression);
        }

        /// <summary>
        /// Apply numeric filter
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="filterData"></param>
        /// <returns></returns>
        public static IQueryable<TModel> ApplyNumericFilter<TModel>(this IQueryable<TModel> query, Expression<Func<TModel, decimal>> property, decimal? filterData)
        {
            if (!filterData.HasValue) return query;

            var propertyExpression = GetPropertyEqualExpression(property, filterData.Value);

            return query.Where(propertyExpression);
        }

        /// <summary>
        /// Get property equal expression
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <param name="filterData"></param>
        /// <returns></returns>
        public static Expression<Func<TModel, bool>> GetPropertyEqualExpression<TModel, TProperty>(Expression<Func<TModel, TProperty>> property, TProperty filterData)
        {
            Expression<Func<TProperty>> filterDataExp = () => filterData;
            var grEx = Expression.Equal(property.Body, filterDataExp.Body);

            return Expression.Lambda<Func<TModel, bool>>(grEx, property.Parameters.Single());
        }


        /// <summary>
        /// Apply numeric filter
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="filterData"></param>
        /// <returns></returns>
        public static IQueryable<TModel> ApplyNumericFilter<TModel>(this IQueryable<TModel> query, Expression<Func<TModel, decimal>> property, string filterData)
        {
            if (!decimal.TryParse(filterData, out decimal filterDataDecimal)) return query;

            return query.ApplyNumericFilter(property, filterDataDecimal);
        }

        /// <summary>
        /// Apply numeric filter
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="filterData"></param>
        /// <returns></returns>
        public static IQueryable<TModel> ApplyNumericFilter<TModel>(this IQueryable<TModel> query, Expression<Func<TModel, int>> property, string filterData)
        {
            if (!int.TryParse(filterData, out int filterDataInt)) return query;

            return query.ApplyNumericFilter(property, filterDataInt);
        }

        /// <summary>
        /// Apply boolean filter
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="filterData"></param>
        /// <returns></returns>
        public static IQueryable<TModel> ApplyBooleanFilter<TModel>(this IQueryable<TModel> query, Expression<Func<TModel, bool>> property, string filterData)
        {
            if (!bool.TryParse(filterData, out bool filterValue)) return query;

            var propertyExpression = GetPropertyEqualExpression(property, filterValue);

            return query.Where(propertyExpression);
        }

        /// <summary>
        /// Apply date range filter
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="dateRangeStr"></param>
        /// <returns></returns>
        public static IQueryable<TModel> ApplyDateRangeFilter<TModel>(this IQueryable<TModel> query, Expression<Func<TModel, DateTime>> property, string dateRangeStr)
        {
            if (string.IsNullOrEmpty(dateRangeStr)) return query;

            var dates = dateRangeStr.Split("-");

            var fromDateStr = dates.FirstOrDefault();
            var toDateStr = dates.ElementAtOrDefault(1);

            if (DateTime.TryParse(fromDateStr, out DateTime fromDate))
            {
                Expression<Func<DateTime>> dateExp = () => fromDate;

                var grEx = Expression.GreaterThanOrEqual(property.Body, dateExp.Body);
                Expression<Func<TModel, bool>> exp = Expression.Lambda<Func<TModel, bool>>(grEx, property.Parameters.Single());

                query = query.Where(exp);
            }

            if (DateTime.TryParse(toDateStr, out DateTime toDate))
            {
                toDate = toDate.AddDays(1);
                Expression<Func<DateTime>> dateExp = () => toDate;

                var ltEx = Expression.LessThan(property.Body, dateExp.Body);
                Expression<Func<TModel, bool>> exp = Expression.Lambda<Func<TModel, bool>>(ltEx, property.Parameters.Single());

                query = query.Where(exp);
            }

            return query;
        }

        /// <summary>
        /// Apply string filter
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static IQueryable<TModel> ApplyStringFilter<TModel>(this IQueryable<TModel> query, Expression<Func<TModel, string>> property, string search)
        {
            if (string.IsNullOrEmpty(search)) return query;

            var searchExp = GetStringFilterExpression(property, search);

            return query.Where(searchExp);
        }

        public static Expression<Func<TModel, bool>> GetStringFilterExpression<TModel>(Expression<Func<TModel, string>> property, string search)
        {
            Expression<Func<string>> searchStrExp = () => search;

            var method = property.Body.Type.GetMethod("Contains", new[] { typeof(string) });

            var conainsExp = Expression.Call(property.Body, method, searchStrExp.Body);

            return Expression.Lambda<Func<TModel, bool>>(conainsExp, property.Parameters.Single());
        }

        /// <summary>
        /// Applay enum filter
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IQueryable<TModel> ApplyEnumFilter<TModel, TEnum>(this IQueryable<TModel> query, Expression<Func<TModel, TEnum?>> property, string options)
            where TEnum : struct, IConvertible
        {
            return query.ApplyOptionsFilter(property, options, i => MapFromStateDto<TEnum>(i));
        }

        /// <summary>
        /// Apply options filter
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="property"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IQueryable<TModel> ApplyOptionsFilter<TModel>(this IQueryable<TModel> query, Expression<Func<TModel, string>> property, string options)
        {
            return query.ApplyOptionsFilter(property, options, i => i);
        }

        public static IQueryable<TModel> ApplyOptionsFilter<TModel, TProperty>(
            this IQueryable<TModel> query,
            Expression<Func<TModel, TProperty>> property,
            string options,
            Expression<Func<string, TProperty>> selector)
        {
            if (string.IsNullOrEmpty(options)) return query;

            var searchExpression = GetOptionsFilterExpression(property, options, selector);

            return query.Where(searchExpression);
        }

        public static Expression<Func<TModel, bool>> GetOptionsFilterExpression<TModel, TProperty>(
            Expression<Func<TModel, TProperty>> property,
            string options,
            Expression<Func<string, TProperty>> selector)
        {
            var types = options.Split("|").AsQueryable().Select(selector);

            var methodInfo = typeof(Enumerable).GetMethods().Where(i => i.Name == "Contains").First();
            var method = methodInfo.MakeGenericMethod(new[] { typeof(TProperty) });

            Expression<Func<IEnumerable<TProperty>>> searchStrExp = () => types;

            var containsExp = Expression.Call(null, method, searchStrExp.Body, property.Body);

            return Expression.Lambda<Func<TModel, bool>>(containsExp, property.Parameters.Single());
        }

        private static TEnum MapFromStateDto<TEnum>(string dtoStatus) where TEnum : struct
        {
            var mapFromStateDto = Enum.Parse<TEnum>(dtoStatus.ToUpperfirstLetter());

            return mapFromStateDto;
        }

        public static IQueryable<TModel> OrderBy<TModel>(this IQueryable<TModel> query, string propertyName, bool descending)
        {
            if (string.IsNullOrEmpty(propertyName)) return query;

            var propertyInfo = typeof(TModel).GetProperties()
                .FirstOrDefault(i => i.Name.ToLower() == propertyName.ToLower());

            if (propertyInfo == null)
            {
                throw new Exception($"Invalid property name {propertyName} for type {typeof(TModel).Name}");
            }

            ParameterExpression param = Expression.Parameter(typeof(TModel), string.Empty);
            MemberExpression property = Expression.PropertyOrField(param, propertyInfo.Name);
            LambdaExpression sort = Expression.Lambda(property, param);
            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                 "OrderBy" + (descending ? "Descending" : string.Empty),
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

        public static IQueryable<TModel> ApplySearch<TModel, TFilter>(
            this IQueryable<TModel> query,
            FilterFormDto<TFilter> searchForm, 
            IEnumerable<Expression<Func<TModel, bool>>> additionalExpressions)
            where TFilter : SearchFilterDto
        {

            if (string.IsNullOrEmpty(searchForm?.Filter?.Search)) return query;

                var properties = typeof(TFilter).GetProperties()
                .Where(i => i.CustomAttributes.Any(attr => attr.AttributeType == typeof(FilterFieldAttribute)));

            if (!properties.Any()) return query;

            var expressions = new List<Expression<Func<TModel, bool>>>();
            ParameterExpression param = Expression.Parameter(typeof(TModel), string.Empty);

            foreach (var property in properties)
            {
                var attr = (FilterFieldAttribute)property.GetCustomAttributes(typeof(FilterFieldAttribute), false).First();
                
                if (!attr.Searched) continue;

                MemberExpression propertyExp = Expression.Property(param, property.Name);

                switch (attr.Type)
                {
                    // String search
                    case FilterFieldType.String:
                        var lambdaStr = Expression.Lambda<Func<TModel, string>>(propertyExp, param);
                        expressions.Add(GetStringFilterExpression(lambdaStr, searchForm.Filter.Search));
                        break;

                    // Integer search
                    case FilterFieldType.Integer:
                        if (!int.TryParse(searchForm.Filter.Search, out int intValue)) continue;

                        var lambdaInt = Expression.Lambda<Func<TModel, int?>>(propertyExp, param);
                        expressions.Add(GetPropertyEqualExpression(lambdaInt, intValue));
                        break;

                    // Decimal search
                    case FilterFieldType.Decimal:
                        if (!decimal.TryParse(searchForm.Filter.Search, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decValue)) continue;

                        var lambdaDec = Expression.Lambda<Func<TModel, decimal?>>(propertyExp, param);
                        expressions.Add(GetPropertyEqualExpression(lambdaDec, decValue));
                        break;

                    // Decimal search
                    //case FilterFieldType.DateRange:
                    //    if (!decimal.TryParse(searchForm.Filter.Search, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decValue)) continue;

                    //    var lambdaDec = Expression.Lambda<Func<TModel, decimal?>>(propertyExp, param);
                    //    expressions.Add(GetPropertyEqualExpression(lambdaDec, decValue));
                    //    break;
                }
            }

            return query.Where(GetOrExpressions(expressions));
        }

        private static Expression<Func<TModel, bool>> GetOrExpressions<TModel>(IEnumerable<Expression<Func<TModel, bool>>> expressions)
        {
            Expression orExpressions = null;
            var param = expressions.First().Parameters.First();

            foreach (var expression in expressions)
            {
                if (orExpressions == null)
                {
                    orExpressions = Expression.Invoke(expression);
                    continue;
                }

                orExpressions = Expression.OrElse(orExpressions, Expression.Invoke(expression));
            }

            return Expression.Lambda<Func<TModel, bool>>(orExpressions, param);
        }

        public static IQueryable<TModel> WhereOr<TModel>(this IQueryable<TModel> query, params Expression<Func<TModel, bool>>[] conditions)
        {
            var exp = GetOrExpressions(conditions);

            return query.Where(exp);
        }
    }
}

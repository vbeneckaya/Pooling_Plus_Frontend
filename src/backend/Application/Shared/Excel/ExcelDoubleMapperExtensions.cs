using Application.Shared.Excel.Columns;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Shared.Excel
{
    public static class ExcelDoubleMapperExtensions
    {
        public static ExcelDoubleMapper<TDto, TInnerDto> MapColumn<TDto, TInnerDto, TField>(this ExcelDoubleMapper<TDto, TInnerDto> excelDoubleMapper, Expression<Func<TDto, TField>> property, IExcelColumn column)
            where TDto : new()
            where TInnerDto : new()
        {
            var propertyBody = property.Body as MemberExpression;
            if (propertyBody != null)
            {
                var propertyInfo = propertyBody.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    column.Property = propertyInfo;
                    excelDoubleMapper.AddColumn(column);
                }
            }
            return excelDoubleMapper;
        }
        
        public static ExcelDoubleMapper<TDto, TInnerDto> MapInnerColumn<TDto, TInnerDto, TField>(this ExcelDoubleMapper<TDto, TInnerDto> excelDoubleMapper, Expression<Func<TInnerDto, TField>> property, IExcelColumn column)
            where TDto : new()
            where TInnerDto : new()
        {
            var propertyBody = property.Body as MemberExpression;
            if (propertyBody != null)
            {
                var propertyInfo = propertyBody.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    column.Property = propertyInfo;
                    excelDoubleMapper.AddInnerColumn(column);
                }
            }
            return excelDoubleMapper;
        }
    }
}

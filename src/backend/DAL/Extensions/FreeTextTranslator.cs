using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DAL.Extensions
{
    public class DateFormatTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(StringExt).GetRuntimeMethod(nameof(StringExt.SqlFormat), new[] { typeof(DateTime), typeof(string) });

        public Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method != _methodInfo) return null;

            var patternExpression = methodCallExpression.Arguments[1];
            var objectExpression = (UnaryExpression)methodCallExpression.Arguments[0];

            var sqlExpression =
                new SqlFunctionExpression("TO_CHAR", typeof(string),
                    new[] { objectExpression, patternExpression });
            return sqlExpression;
        }
    }
}

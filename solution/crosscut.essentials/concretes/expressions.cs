using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace reexmonkey.crosscut.essentials.concretes
{
    public static class LambdaExpressionExtensions
    {

        public static string GetMemberName<T>(Expression<Func<T>> expression)
        {
            return (expression != null && expression.Body != null) ? (expression.Body as MemberExpression).Member.Name : string.Empty;
        }

    }
}

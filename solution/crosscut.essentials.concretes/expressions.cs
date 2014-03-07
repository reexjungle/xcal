using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace reexmonkey.crosscut.essentials.concretes
{
    public static class LambdaExpressionExtensions
    {
        public static string GetMemberName(this LambdaExpression expression)
        {
            Func<Expression, string> selector = null;  //recursive func
            selector = e => //or move the entire thing to a separate recursive method
            {
                switch (e.NodeType)
                {
                    case ExpressionType.Parameter: return ((ParameterExpression)e).Name;
                    case ExpressionType.MemberAccess: return ((MemberExpression)e).Member.Name;
                    case ExpressionType.Call: return ((MethodCallExpression)e).Method.Name;
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked: return selector(((UnaryExpression)e).Operand);
                    case ExpressionType.Invoke:  return selector(((InvocationExpression)e).Expression);
                    case ExpressionType.ArrayLength: return "Length";
                    default:
                        throw new Exception("not a proper member selector");
                }
            };

            return selector(expression.Body);
        }

        public static IEnumerable<string> GetMemberNames(this LambdaExpression expression)
        {
            Func<Expression, IEnumerable<string>> selector = null;  //recursive func
            selector = e => //or move the entire thing to a separate recursive method
            {
                switch (e.NodeType)
                {
                    case ExpressionType.Parameter: return ((ParameterExpression)e).Name.ToSingleton();
                    case ExpressionType.MemberAccess: return ((MemberExpression)e).Member.Name.ToSingleton();
                    case ExpressionType.New: return ((NewExpression)e).Members.Select(x => x.Name);
                    case ExpressionType.Call: return ((MethodCallExpression)e).Method.Name.ToSingleton();
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked: return selector(((UnaryExpression)e).Operand);
                    case ExpressionType.Invoke: return selector(((InvocationExpression)e).Expression);
                    case ExpressionType.ArrayLength: return "Length".ToSingleton();
                    default:
                        throw new Exception("not a proper member selector");
                }
            };

            return selector(expression.Body);
        }


    }
}

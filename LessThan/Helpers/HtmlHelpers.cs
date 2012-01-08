using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace LessThan.Helpers
{
    public static class HtmlHelpers
    {
        public static string GetHtmlIdFor<TModel, TValue>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression)
        {
            return (GetHtmlIdFor(expression));
        }

        private static string GetHtmlIdFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCallExpression = (MethodCallExpression)expression.Body;
                string name = GetHtmlIdFor(methodCallExpression);
                return name.Substring(expression.Parameters[0].Name.Length + 1).Replace('.', '_');

            }
            return expression.Body.ToString().Substring(expression.Parameters[0].Name.Length + 1).Replace('.', '_');
        }

        private static string GetHtmlIdFor(MethodCallExpression expression)
        {
            var methodCallExpression = expression.Object as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return GetHtmlIdFor(methodCallExpression);
            }

            return expression.Object.ToString();
        }
    }
}
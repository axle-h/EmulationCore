﻿namespace Axh.Emulation.CPU.X80.Util
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class ExpressionHelpers
    {
        public static MemberExpression GetPropertyExpression<TSource, TProperty>(this Expression instance, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", propertyLambda));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", propertyLambda));
            }

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expresion '{0}' refers to a property that is not from type {1}.", propertyLambda, type));
            }
            
            return Expression.Property(instance, propInfo);
        }

        public static MethodCallExpression GetMethodExpression<TSource, TArg0, TResult>(this Expression instance, Expression<Func<TSource, TArg0, TResult>> methodLambda, Expression arg0)
        {
            var outermostExpression = methodLambda.Body as MethodCallExpression;

            if (outermostExpression == null)
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            var methodInfo = outermostExpression.Method;

            return Expression.Call(instance, methodInfo, new[] { arg0 });
        }

        public static MethodInfo GetMethodInfo<TSource, TArg, TResult>(Expression<Func<TSource, TArg, TResult>> methodLambda)
        {
            var outermostExpression = methodLambda.Body as MethodCallExpression;

            if (outermostExpression == null)
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            return outermostExpression.Method;
        }
    }
}

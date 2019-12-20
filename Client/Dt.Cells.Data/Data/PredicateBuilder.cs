#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            InvocationExpression expression = Expression.Invoke((Expression) expr2, Enumerable.Cast<Expression>((IEnumerable) expr1.Parameters));
            return Expression.Lambda<Func<T, bool>>((Expression) Expression.AndAlso(expr1.Body, (Expression) expression), (IEnumerable<ParameterExpression>) expr1.Parameters);
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return Expression.Lambda<Func<T, bool>>((Expression) Expression.Constant((bool) false, (Type) typeof(bool)), new ParameterExpression[] { Expression.Parameter((Type) typeof(T), "f") });
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            InvocationExpression expression = Expression.Invoke((Expression) expr2, Enumerable.Cast<Expression>((IEnumerable) expr1.Parameters));
            return Expression.Lambda<Func<T, bool>>((Expression) Expression.OrElse(expr1.Body, (Expression) expression), (IEnumerable<ParameterExpression>) expr1.Parameters);
        }

        public static Expression<Func<T, bool>> True<T>()
        {
            return Expression.Lambda<Func<T, bool>>((Expression) Expression.Constant((bool) true, (Type) typeof(bool)), new ParameterExpression[] { Expression.Parameter((Type) typeof(T), "f") });
        }
    }
}


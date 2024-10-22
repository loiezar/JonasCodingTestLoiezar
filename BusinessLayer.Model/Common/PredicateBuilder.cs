using System;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLayer.Model.Common
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
        public static Expression<Func<T, bool>> AndDateRange<T>(this Expression<Func<T, bool>> expr1,
                                                                string propertyName, DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            var parameterExpression = Expression.Parameter(typeof(T));
            Expression body = parameterExpression;
            foreach (var property in propertyName.Split('.'))
            {
                body = Expression.PropertyOrField(body, property);
            }

            BinaryExpression comparisonDateTimeFrom;
            try
            {
                comparisonDateTimeFrom = Expression.GreaterThanOrEqual(body, Expression.Constant(dateTimeFrom, typeof(DateTime)));
            }
            catch (Exception)
            {
                comparisonDateTimeFrom = Expression.GreaterThanOrEqual(body, Expression.Constant(dateTimeFrom, typeof(DateTime?)));
            }

            var lambdaDateTimeFrom = Expression.Lambda<Func<T, bool>>(comparisonDateTimeFrom, parameterExpression);
            var invokedExprDateTimeFrom = Expression.Invoke(lambdaDateTimeFrom, expr1.Parameters.Cast<Expression>());

            BinaryExpression comparisonDateTimeTo;
            try
            {
                comparisonDateTimeTo = Expression.LessThanOrEqual(body, Expression.Constant(dateTimeTo, typeof(DateTime)));
            }
            catch (Exception)
            {
                comparisonDateTimeTo = Expression.LessThanOrEqual(body, Expression.Constant(dateTimeTo, typeof(DateTime?)));
            }

            var lambdaDateTimeTo = Expression.Lambda<Func<T, bool>>(comparisonDateTimeTo, parameterExpression);
            var invokedExprDateTimeTo = Expression.Invoke(lambdaDateTimeTo, expr1.Parameters.Cast<Expression>());

            var newExpression = Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExprDateTimeFrom), expr1.Parameters);

            var newExpression2 = Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(newExpression.Body, invokedExprDateTimeTo), newExpression.Parameters);

            return newExpression2;
        }
    }
}

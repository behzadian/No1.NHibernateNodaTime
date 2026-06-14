namespace No1.NHibernateNodaTime;

using System;
using System.Linq.Expressions;

internal static class ReflectionUtility
{
	internal static Expression<Func<T, object>> GetPropertExpression<T>(string propertyName) {
		if (string.IsNullOrEmpty(propertyName)) {
			throw new ArgumentException("propertyName must be provided", nameof(propertyName));
		}

		var param = Expression.Parameter(typeof(T), "x");
		var body = Expression.PropertyOrField(param, propertyName);

		// Ensure the expression returns object (box value types)
		var converted = Expression.Convert(body, typeof(object));

		return Expression.Lambda<Func<T, object>>(converted, param);
	}
}
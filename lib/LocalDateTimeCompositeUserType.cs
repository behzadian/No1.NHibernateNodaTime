using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;
using NodaTime;
using NodaTime.Calendars;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

/// <summary>
/// </summary>
public sealed class LocalDateTimeCompositeUserType : ICompositeUserType
{
	Type ICompositeUserType.ReturnedClass => typeof(LocalDateTime?);

	bool ICompositeUserType.IsMutable => false;

	internal static string[] Columns = [.. LocalDateCompositeUserType.Columns, ..LocalTimeUserType.Columns];
	internal static int DateColumnsCount => LocalDateCompositeUserType.Columns.Length;

	string[] ICompositeUserType.PropertyNames => Columns;


	IType[] ICompositeUserType.PropertyTypes =>
	[
		..LocalDateCompositeUserType.Instance.PropertyTypes,
		LocalTimeUserType.NHType
	];

	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
	{
		// Split names between date and time parts
		var dateNames = names[..DateColumnsCount];
		var timeName = names[DateColumnsCount..];

		var date = (LocalDate?)LocalDateCompositeUserType.Instance.NullSafeGet(dr, dateNames, session, owner);

		var time = (LocalTime?)LocalTimeUserType.Instance.NullSafeGet(dr, timeName, session, owner);

		if (date is null || time is null) return null;

		return date.Value + time.Value;
	}

	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session)
	{
		if (value is LocalDateTime ldt)
		{
			LocalDateCompositeUserType.Instance.NullSafeSet(cmd, ldt.Date, index, settable, session);
			LocalTimeUserType.Instance.NullSafeSet(cmd, ldt.TimeOfDay, index + DateColumnsCount, session);
		}
		else
		{
			LocalDateCompositeUserType.Instance.NullSafeSet(cmd, null, index, settable, session);
			LocalTimeUserType.Instance.NullSafeSet(cmd, null, index + DateColumnsCount, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property)
	{
		if (component is LocalDateTime ldt)
		{
			return property < DateColumnsCount ? LocalDateCompositeUserType.Instance.GetPropertyValue(ldt.Date, property) : ldt.TimeOfDay;
		}
		else
		{
			throw new MismatchTypeException($"Object is not LocalDateTime, is {component?.GetType()?.Name ?? "NULL"}");
		}
	}

	void ICompositeUserType.SetPropertyValue(object component, int property, object value)
	{
		throw new InvalidOperationException("immutable");
	}

	object ICompositeUserType.DeepCopy(object value)
	{
		return value;
	}

	object ICompositeUserType.Disassemble(object value, ISessionImplementor session)
	{
		return value;
	}

	object ICompositeUserType.Assemble(object cached, ISessionImplementor session, object owner)
	{
		return cached;
	}

	object ICompositeUserType.Replace(object original, object target, ISessionImplementor session, object owner)
	{
		return original;
	}

	bool ICompositeUserType.Equals(object? x, object? y)
	{
		if (ReferenceEquals(x, y)) return true;
		if (x == null || y == null) return false;
		return ((LocalDateTime)x).Equals((LocalDateTime)y);
	}

	int ICompositeUserType.GetHashCode(object? x)
	{
		return x?.GetHashCode() ?? 0;
	}
}
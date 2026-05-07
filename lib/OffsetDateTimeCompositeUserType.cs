using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;
using NodaTime;
using System.Data.Common;
using static No1.NHibernateNodaTime.NodaTimeUtility;


namespace No1.NHibernateNodaTime;

/// <summary>
/// </summary>
public sealed class OffsetDateTimeCompositeUserType : ICompositeUserType
{
	Type ICompositeUserType.ReturnedClass => typeof(OffsetDateTime?);

	bool ICompositeUserType.IsMutable => false;

	internal static string[] Columns = [.. LocalDateTimeCompositeUserType.Columns, ..OffsetUserType.Columns];
	internal static int DateTimeColumnsCount => LocalDateTimeCompositeUserType.Columns.Length;

	string[] ICompositeUserType.PropertyNames => Columns;


	IType[] ICompositeUserType.PropertyTypes =>
	[
		..LocalDateTimeCompositeUserType.Instance.PropertyTypes,
		OffsetUserType.NHType
	];

	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
	{
		// Split names between date and time parts
		var dateNames = names[..DateTimeColumnsCount];
		var timeName = names[DateTimeColumnsCount..];

		var date = (LocalDateTime?)LocalDateTimeCompositeUserType.Instance.NullSafeGet(dr, dateNames, session, owner);

		var offset = (Offset?)OffsetUserType.Instance.NullSafeGet(dr, timeName, session, owner);

		if (date is null || offset is null) return null;

		return new OffsetDateTime(date.Value, offset.Value);
	}

	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session)
	{
		if (value is OffsetDateTime val)
		{
			LocalDateTimeCompositeUserType.Instance.NullSafeSet(cmd, val.LocalDateTime, index, settable, session);
			OffsetUserType.Instance.NullSafeSet(cmd, val.Offset, index + DateTimeColumnsCount, session);
		}
		else
		{
			LocalDateTimeCompositeUserType.Instance.NullSafeSet(cmd, null, index, settable, session);
			OffsetUserType.Instance.NullSafeSet(cmd, null, index + DateTimeColumnsCount, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property)
	{
		if (component is OffsetDateTime val)
		{
			return property < DateTimeColumnsCount ? LocalDateTimeCompositeUserType.Instance.GetPropertyValue(val.LocalDateTime, property) : val.Offset;
		}
		else
		{
			throw new MismatchTypeException($"Object is not OffsetDateTime, is {component?.GetType()?.Name ?? "NULL"}");
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
		return ((OffsetDateTime)x).Equals((OffsetDateTime)y);
	}

	int ICompositeUserType.GetHashCode(object? x)
	{
		return x?.GetHashCode() ?? 0;
	}
}
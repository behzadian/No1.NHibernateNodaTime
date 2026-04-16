using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;
using System.Diagnostics.Metrics;
using static No1.NHibernateNodaTime.NodaTimeUtility;

namespace No1.NHibernateNodaTime;

/// <summary>
/// Composite UserType that stores NodaTime Instant as two separate columns:
/// - Seconds since Unix epoch (long)
/// - Nanoseconds component (int)
/// </summary>
public sealed class InstantCompositeUserType : ICompositeUserType
{
	Type ICompositeUserType.ReturnedClass => typeof(Instant?);

	bool ICompositeUserType.IsMutable => false;

	internal static string[] Columns => ["Seconds", "Nanoseconds", "Timestamp",];

	string[] ICompositeUserType.PropertyNames => Columns;

	IType[] ICompositeUserType.PropertyTypes =>
	[
		NHibernateUtil.Int64,			// Seconds
        NHibernateUtil.Int32,			// Nanoseconds
		NHibernateUtil.DateTimeNoMs,	// Timestamp
    ];

	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
	{
		var index = 0;

		if (dr[names[index++]] is not long secs)
			return null;

		if (dr[names[index++]] is not int nanos)
			return null;

		return Instant.FromUnixTimeSeconds(secs).PlusNanoseconds(nanos);
	}

	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session)
	{
		if (value is Instant instant)
		{
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, instant.ToUnixTimeSecondsAndNanoseconds().seconds, counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds, counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, TryOrDefault(instant.ToDateTimeUtc), counter++, session);
		}
		else
		{
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property)
	{
		var instant = (Instant)component;
		return property switch
		{
			0 => instant.ToUnixTimeSecondsAndNanoseconds().seconds,
			1 => instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds,
			2 => TryOrDefault(instant.ToDateTimeUtc),
			_ => throw new ArgumentOutOfRangeException(nameof(property))
		};
	}

	void ICompositeUserType.SetPropertyValue(object component, int property, object value)
	{
		throw new InvalidOperationException("Instant is immutable");
	}

	object ICompositeUserType.DeepCopy(object value)
	{
		// Instant is immutable
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
		return ((Instant)x).Equals((Instant)y);
	}

	int ICompositeUserType.GetHashCode(object? x)
	{
		return x?.GetHashCode() ?? 0;
	}
}
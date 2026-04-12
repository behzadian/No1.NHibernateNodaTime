using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;
using System.Diagnostics.Metrics;

namespace No1.NHibernateNodaTime;

/// <summary>
/// Composite UserType that stores NodaTime Instant as two separate columns:
/// - Seconds since Unix epoch (long)
/// - Nanoseconds component (int)
/// </summary>
public class InstantCompositeUserType : ICompositeUserType
{
	public Type ReturnedClass => typeof(Instant?);

	public bool IsMutable => false;

	internal static string[] Columns => ["Seconds", "Nanoseconds", "Timestamp",];

	public string[] PropertyNames => Columns;

	public IType[] PropertyTypes =>
	[
		NHibernateUtil.Int64,			// Seconds
        NHibernateUtil.Int32,			// Nanoseconds
		NHibernateUtil.UtcDateTimeNoMs,	// Timestamp
    ];

	public object? NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
	{
		var index = 0;

		if (dr[names[index++]] is not long secs)
			return null;

		if (dr[names[index++]] is not int nanos)
			return null;

		return Instant.FromUnixTimeSeconds(secs).PlusNanoseconds(nanos);
	}

	public void NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session)
	{
		if (value is Instant instant)
		{
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, instant.ToUnixTimeSecondsAndNanoseconds().seconds, counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds, counter++, session);
			NHibernateUtil.UtcDateTimeNoMs.NullSafeSet(cmd, instant.ToDateTimeUtcOrNull(), counter++, session);
		}
		else
		{
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.UtcDateTimeNoMs.NullSafeSet(cmd, null, counter++, session);
		}
	}

	public object? GetPropertyValue(object component, int property)
	{
		var instant = (Instant)component;
		return property switch
		{
			0 => instant.ToUnixTimeSecondsAndNanoseconds().seconds,
			1 => instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds,
			2 => instant.ToDateTimeUtcOrNull(),
			_ => throw new ArgumentOutOfRangeException(nameof(property))
		};
	}

	public void SetPropertyValue(object component, int property, object value)
	{
		throw new InvalidOperationException("Instant is immutable");
	}

	public object DeepCopy(object value)
	{
		// Instant is immutable
		return value;
	}

	public object Disassemble(object value, ISessionImplementor session)
	{
		return value;
	}

	public object Assemble(object cached, ISessionImplementor session, object owner)
	{
		return cached;
	}

	public object Replace(object original, object target, ISessionImplementor session, object owner)
	{
		return original;
	}

	public new bool Equals(object? x, object? y)
	{
		if (ReferenceEquals(x, y)) return true;
		if (x == null || y == null) return false;
		return ((Instant)x).Equals((Instant)y);
	}

	public int GetHashCode(object? x)
	{
		return x?.GetHashCode() ?? 0;
	}
}
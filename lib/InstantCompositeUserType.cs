using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

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

	public string[] PropertyNames => ["Timestamp", "Nanoseconds"];

	public IType[] PropertyTypes =>
	[
		NHibernateUtil.UtcDateTimeNoMs,	// Timestamp
        NHibernateUtil.Int32			// Nanoseconds
    ];

	public object? NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
	{
		if (dr[names[0]] is not DateTime utc)
			return null;

		if (dr[names[1]] is not int nanos)
			return null;

		return Instant.FromDateTimeUtc(NodaTimeUtility.AsUtc(utc)).PlusNanoseconds(nanos);
	}

	public void NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session)
	{
		if (value is Instant instant)
		{
			NHibernateUtil.UtcDateTimeNoMs.NullSafeSet(cmd, instant.ToDateTimeUtc(), index, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds, index + 1, session);
		}
		else
		{
			NHibernateUtil.UtcDateTimeNoMs.NullSafeSet(cmd, null, index, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, null, index + 1, session);
		}
	}

	public object GetPropertyValue(object component, int property)
	{
		var instant = (Instant)component;
		return property switch
		{
			0 => instant.ToDateTimeUtc(),
			1 => instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds,
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
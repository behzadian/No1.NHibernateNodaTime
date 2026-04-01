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
public class ZonedDateTimeCompositeUserType : ICompositeUserType
{
	public Type ReturnedClass => typeof(ZonedDateTime?);

	public bool IsMutable => false;

	public string[] PropertyNames => ["UTC", "Local", "Nanoseconds", "ZoneID"];

	public IType[] PropertyTypes =>
	[
		NHibernateUtil.DateTimeNoMs,// Utc
		NHibernateUtil.DateTimeNoMs,// Local
		NHibernateUtil.Int32,		// Nanoseconds
		NHibernateUtil.String,		// ZoneID
	];

	public object? NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
	{
		if (dr[names[0]] is not DateTime utc)
			return null;

		if (dr[names[1]] is not DateTime local)
			return null;

		if (dr[names[2]] is not int nanos)
			return null;

		if (dr[names[3]] is not string zoneId)
			return null;

		var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) ?? throw new Exception($"Zone {zoneId} not found");
		var instant = Instant.FromDateTimeUtc(NodaTimeUtility.AsUtc(utc)).PlusNanoseconds(nanos);
		var zdt = instant.InZone(DateTimeZone.Utc);
		return zdt.WithZone(zone);
	}

	public void NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session)
	{
		if (value is ZonedDateTime zdt)
		{
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, zdt.ToDateTimeUtc(), index, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, zdt.ToDateTimeUnspecified(), index + 1, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, zdt.ToInstant().ToUnixTimeSecondsAndNanoseconds().nanoseconds, index + 2, session);
			NHibernateUtil.String.NullSafeSet(cmd, zdt.Zone.Id, index + 3, session);
		}
		else
		{
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, null, index, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, null, index + 1, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, null, index + 2, session);
			NHibernateUtil.String.NullSafeSet(cmd, null, index + 3, session);
		}
	}

	public object GetPropertyValue(object component, int property)
	{
		var val = (ZonedDateTime)component;
		return property switch
		{
			0 => val.ToDateTimeUtc(),
			1 => val.ToDateTimeUnspecified(),
			2 => val.ToInstant().ToUnixTimeSecondsAndNanoseconds().nanoseconds,
			3 => val.Zone.Id,
			_ => throw new ArgumentOutOfRangeException(nameof(property))
		};
	}

	public void SetPropertyValue(object component, int property, object value)
	{
		throw new InvalidOperationException("immutable");
	}

	public object DeepCopy(object value)
	{
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
		return ((ZonedDateTime)x).Equals((ZonedDateTime)y);
	}

	public int GetHashCode(object? x)
	{
		return x?.GetHashCode() ?? 0;
	}
}
using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

/// <summary>
/// </summary>
public class LocalTimeCompositeUserType : ICompositeUserType
{
	public Type ReturnedClass => typeof(ZonedDateTime?);

	public bool IsMutable => false;

	internal static string[] Columns => ["Seconds", "Nanoseconds", "ZoneID", "UTC", "Local",];

	public string[] PropertyNames => Columns;

	public IType[] PropertyTypes =>
	[
		NHibernateUtil.Int64,		// Seconds
		NHibernateUtil.Int32,		// Nanoseconds
		NHibernateUtil.String,		// ZoneID
		NHibernateUtil.DateTimeNoMs,// Utc
		NHibernateUtil.DateTimeNoMs,// Local
	];

	public object? NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
	{
		var counter = 0;

		if (dr[names[counter++]] is not long secs)
			return null;

		if (dr[names[counter++]] is not int nanos)
			return null;

		if (dr[names[counter++]] is not string zoneId)
			return null;

		var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) ?? throw new Exception($"Zone {zoneId} not found");
		var instant = Instant.FromUnixTimeSeconds(secs).PlusNanoseconds(nanos);
		var zdt = instant.InZone(DateTimeZone.Utc);
		return zdt.WithZone(zone);
	}

	public void NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session)
	{
		if (value is ZonedDateTime zdt)
		{
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, zdt.ToInstant().ToUnixTimeSeconds(), counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, zdt.ToInstant().ToUnixTimeSecondsAndNanoseconds().nanoseconds, counter++, session);
			NHibernateUtil.String.NullSafeSet(cmd, zdt.Zone.Id, counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, zdt.ToDateTimeUtcOrNull(), counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, zdt.ToDateTimeUnspecifiedOrNull(), counter++, session);
		}
		else
		{
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.String.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, null, counter++, session);
		}
	}

	public object? GetPropertyValue(object component, int property)
	{
		var val = (ZonedDateTime)component;
		return property switch
		{
			0 => val.ToInstant().ToUnixTimeSecondsAndNanoseconds().seconds,
			1 => val.ToInstant().ToUnixTimeSecondsAndNanoseconds().nanoseconds,
			2 => val.Zone.Id,
			3 => val.ToDateTimeUtcOrNull(),
			4 => val.ToDateTimeUnspecifiedOrNull(),
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
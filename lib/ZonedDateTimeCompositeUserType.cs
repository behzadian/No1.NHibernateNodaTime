using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

using static No1.NHibernateNodaTime.NodaTimeUtility;

namespace No1.NHibernateNodaTime;

public sealed class ZonedDateTimeCompositeUserType : ICompositeUserType
{
	internal static readonly string[] Columns = ["Seconds", "Nanoseconds", "ZoneID", "UTC", "Local",];

	Type ICompositeUserType.ReturnedClass => typeof(ZonedDateTime?);

	bool ICompositeUserType.IsMutable => false;

	string[] ICompositeUserType.PropertyNames => Columns;

	IType[] ICompositeUserType.PropertyTypes =>
	[
		NHibernateUtil.Int64,       // Seconds
		NHibernateUtil.Int32,       // Nanoseconds
		NHibernateUtil.String,      // ZoneID
		NHibernateUtil.DateTimeNoMs, // Utc
		NHibernateUtil.DateTimeNoMs, // Local
	];

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulnes")]
	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner) {
		var counter = 0;

		if (dr[names[counter++]] is not long secs) {
			return null;
		}

		if (dr[names[counter++]] is not int nanos) {
			return null;
		}

		if (dr[names[counter++]] is not string zoneId) {
			return null;
		}

		var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) ?? throw new UnsupportedValueException(zoneId);
		var instant = Instant.FromUnixTimeSeconds(secs).PlusNanoseconds(nanos);
		var zdt = instant.InZone(DateTimeZone.Utc);
		return zdt.WithZone(zone);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulnes")]
	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session) {
		if (value is ZonedDateTime zdt) {
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, zdt.ToInstant().ToUnixTimeSeconds(), counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, zdt.ToInstant().ToUnixTimeSecondsAndNanoseconds().nanoseconds, counter++, session);
			NHibernateUtil.String.NullSafeSet(cmd, zdt.Zone.Id, counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, TryOrDefault(zdt.ToDateTimeUtc), counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, TryOrDefault(zdt.ToDateTimeUnspecified), counter++, session);
		} else {
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.String.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property) {
		if (component is ZonedDateTime val) {
			return property switch {
				0 => val.ToInstant().ToUnixTimeSecondsAndNanoseconds().seconds,
				1 => val.ToInstant().ToUnixTimeSecondsAndNanoseconds().nanoseconds,
				2 => val.Zone.Id,
				3 => TryOrDefault(val.ToDateTimeUtc),
				4 => TryOrDefault(val.ToDateTimeUnspecified),
				_ => throw new ArgumentOutOfRangeException(nameof(property)),
			};
		} else {
			throw new UnexpectedTypeException<ZonedDateTime>(component);
		}
	}

	void ICompositeUserType.SetPropertyValue(object component, int property, object value) {
		throw new InvalidOperationException("immutable");
	}

	object ICompositeUserType.DeepCopy(object value) {
		return value;
	}

	object ICompositeUserType.Disassemble(object value, ISessionImplementor session) {
		return value;
	}

	object ICompositeUserType.Assemble(object cached, ISessionImplementor session, object owner) {
		return cached;
	}

	object ICompositeUserType.Replace(object original, object target, ISessionImplementor session, object owner) {
		return original;
	}

	bool ICompositeUserType.Equals(object? x, object? y) {
		if (ReferenceEquals(x, y)) {
			return true;
		}

		if (x == null || y == null) {
			return false;
		}

		return ((ZonedDateTime)x).Equals((ZonedDateTime)y);
	}

	int ICompositeUserType.GetHashCode(object? x) {
		return x?.GetHashCode() ?? 0;
	}
}
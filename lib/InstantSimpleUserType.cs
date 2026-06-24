using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;
using static No1.NHibernateNodaTime.NodaTimeUtility;

namespace No1.NHibernateNodaTime;

/// <summary>
/// Composite UserType that stores NodaTime Instant as two separate columns:
/// - Seconds since Unix epoch (long).
/// - Nanoseconds component (int).
/// </summary>
public sealed class InstantSimpleUserType : IUserType
{
	internal static readonly string[] Columns = ["Timestamp",];

	SqlType[] IUserType.SqlTypes => [NHibernateUtil.UtcDbTimestamp.SqlType];

	Type IUserType.ReturnedType => typeof(Instant);

	bool IUserType.IsMutable => false;


	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulness")]
	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session) {
		if (value is Instant instant) {
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, instant.ToUnixTimeSecondsAndNanoseconds().seconds, counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds, counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, TryOrDefault(instant.ToDateTimeUtc), counter++, session);
		} else {
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property) {
		if (component is Instant val) {
			return property switch {
				0 => val.ToUnixTimeSecondsAndNanoseconds().seconds,
				1 => val.ToUnixTimeSecondsAndNanoseconds().nanoseconds,
				2 => TryOrDefault(val.ToDateTimeUtc),
				_ => throw new ArgumentOutOfRangeException(nameof(property)),
			};
		} else {
			throw new UnexpectedTypeException<Instant>(component);
		}
	}

	void ICompositeUserType.SetPropertyValue(object component, int property, object value) {
		throw new InvalidOperationException("Instant is immutable");
	}

	object ICompositeUserType.DeepCopy(object value) {
		// Instant is immutable
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

		return ((Instant)x).Equals((Instant)y);
	}

	int ICompositeUserType.GetHashCode(object? x) {
		return x?.GetHashCode() ?? 0;
	}

	bool IUserType.Equals(object x, object y) {
		throw new NotImplementedException();
	}

	int IUserType.GetHashCode(object x) {
		throw new NotImplementedException();
	}

	object IUserType.NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner) {
		throw new NotImplementedException();
	}

	void IUserType.NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session) {
		throw new NotImplementedException();
	}

	object IUserType.DeepCopy(object value) {
		throw new NotImplementedException();
	}

	object IUserType.Replace(object original, object target, object owner) {
		throw new NotImplementedException();
	}

	object IUserType.Assemble(object cached, object owner) {
		throw new NotImplementedException();
	}

	object IUserType.Disassemble(object value) {
		throw new NotImplementedException();
	}
}
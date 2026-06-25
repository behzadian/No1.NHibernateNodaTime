using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

public sealed class InstantCompactUserType : IUserType
{
	internal static readonly string[] Columns = ["Timestamp",];

	SqlType[] IUserType.SqlTypes => [NHibernateUtil.UtcDbTimestamp.SqlType];

	Type IUserType.ReturnedType => typeof(Instant);

	bool IUserType.IsMutable => false;

	bool IUserType.Equals(object x, object y) {
		if (ReferenceEquals(x, y)) {
			return true;
		}

		if (x == null || y == null) {
			return false;
		}

		if (x is not Instant || y is not Instant) {
			return false;
		}

		return ((Instant)x).Equals((Instant)y);
	}

	int IUserType.GetHashCode(object x) {
		return x?.GetHashCode() ?? 0;
	}

	object? IUserType.NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner) {
		if (rs[names[0]] is not DateTime dateTime) {
			return null;
		}

		return Instant.FromDateTimeUtc(new DateTime(dateTime.Ticks, DateTimeKind.Utc));
	}

	void IUserType.NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session) {
		if (value is Instant instant) {
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, instant.ToDateTimeUtc(), index, session);
		} else {
			NHibernateUtil.DateTimeNoMs.NullSafeSet(cmd, null, index, session);
		}
	}

	object IUserType.DeepCopy(object value) {
		return value;
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
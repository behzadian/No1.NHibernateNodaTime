using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

public sealed class DurationCompactUserType : IUserType
{
	bool IUserType.IsMutable => false;

	SqlType[] IUserType.SqlTypes => [NHibernateUtil.Int64.SqlType];

	Type IUserType.ReturnedType => typeof(Duration?);

	object IUserType.Assemble(object cached, object owner) {
		throw new NotImplementedException();
	}

	object IUserType.DeepCopy(object value) {
		return value;
	}

	object IUserType.Disassemble(object value) {
		throw new NotImplementedException();
	}

	bool IUserType.Equals(object x, object y) {
		return x?.Equals(y) ?? false;
	}

	int IUserType.GetHashCode(object x) {
		return x?.GetHashCode() ?? 0;
	}

	object? IUserType.NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner) {
		if (rs[names[0]] is not long millis) {
			return null;
		}

		return Duration.FromMilliseconds(millis);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulness")]
	void IUserType.NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session) {
		if (value is Duration duration) {
			var millis = (long)(duration.ToInt128Nanoseconds() / (long)Math.Pow(10, 6));
			NHibernateUtil.Int64.NullSafeSet(cmd, millis, index, session);
		} else {
			NHibernateUtil.Int64.NullSafeSet(cmd, null, index, session);
		}
	}

	object IUserType.Replace(object original, object target, object owner) {
		throw new NotImplementedException();
	}
}
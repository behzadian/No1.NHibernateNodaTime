using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

public sealed class OffsetUserType : IUserType
{
	internal static readonly IUserType Instance = new OffsetUserType();

	internal static readonly IType NHType = NHibernateUtil.Custom(typeof(OffsetUserType));

	internal static readonly string[] Columns = ["OffsetNanos"];

	SqlType[] IUserType.SqlTypes => [NHibernateUtil.Int64.SqlType];

	Type IUserType.ReturnedType => typeof(Offset?);

	bool IUserType.IsMutable => false;

	bool IUserType.Equals(object x, object y) {
		if (ReferenceEquals(x, y)) {
			return true;
		}

		if (x == null || y == null) {
			return false;
		}

		return ((Offset)x).Equals((Offset)y);
	}

	int IUserType.GetHashCode(object x) {
		return x?.GetHashCode() ?? 0;
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulnes")]
	object? IUserType.NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner) {
		var counter = 0;

		if (rs[names[counter++]] is not long nanos) {
			return null;
		}

		return Offset.FromNanoseconds(nanos);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulnes")]
	void IUserType.NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session) {
		if (value is Offset offset) {
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, offset.Nanoseconds, counter++, session);
		} else {
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object IUserType.DeepCopy(object value) {
		return value;
	}

	object IUserType.Replace(object original, object target, object owner) {
		return original;
	}

	object IUserType.Assemble(object cached, object owner) {
		return cached;
	}

	object IUserType.Disassemble(object value) {
		return value;
	}
}
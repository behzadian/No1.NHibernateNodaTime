using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

public sealed class PeriodCompactUserType : IUserType
{
	bool IUserType.IsMutable => false;

	SqlType[] IUserType.SqlTypes => throw new NotImplementedException();

	Type IUserType.ReturnedType => throw new NotImplementedException();

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulnes")]
	object? IUserType.NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner) {
		var value = rs.GetString(0);
		var parts = value.Split(".,:".ToCharArray()).Select(int.Parse).ToArray();
		if (parts.Length != 5) {
			throw new UnsupportedValueException(value, "Compact period");
		}

		var periodBuilder = new PeriodBuilder() {
			Years = parts[0],
			Months = parts[1],
			Weeks = parts[2],
			Days = parts[3],
			Nanoseconds = parts[4],
		};

		return periodBuilder.Build();
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulnes")]
	void IUserType.NullSafeSet(DbCommand cmd, object? value, int index, ISessionImplementor session) {
		if (value is Period val) {
			string textual = $"{val.Years},{val.Months},{val.Weeks},{val.Days},{val.Nanoseconds}";
			NHibernateUtil.String.NullSafeSet(cmd, textual, index, session);
		} else {
			NHibernateUtil.Int16.NullSafeSet(cmd, null, index, session);
		}
	}

	object IUserType.DeepCopy(object value) {
		return value;
	}

	bool IUserType.Equals(object? x, object? y) {
		if (ReferenceEquals(x, y)) {
			return true;
		}

		if (x == null || y == null) {
			return false;
		}

		return ((Period)x).Equals((Period)y);
	}

	int IUserType.GetHashCode(object? x) {
		return x?.GetHashCode() ?? 0;
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
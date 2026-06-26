using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

public sealed class PeriodCompleteUserType : ICompositeUserType
{
	internal static readonly string[] Columns = ["Years", "Months", "Weeks", "Days", "Nanos",];

	Type ICompositeUserType.ReturnedClass => typeof(Period);

	bool ICompositeUserType.IsMutable => false;

	string[] ICompositeUserType.PropertyNames => Columns;

	IType[] ICompositeUserType.PropertyTypes =>
	[
		NHibernateUtil.Int16,       // Years
		NHibernateUtil.Int16,       // Months
		NHibernateUtil.Int16,       // Weeks
		NHibernateUtil.Int16,       // Days
		NHibernateUtil.Int64,       // Nanos
	];

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulnes")]
	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner) {
		var counter = 0;

		if (dr[names[counter++]] is not int years) {
			return null;
		}

		if (dr[names[counter++]] is not int months) {
			return null;
		}

		if (dr[names[counter++]] is not int weeks) {
			return null;
		}

		if (dr[names[counter++]] is not int days) {
			return null;
		}

		if (dr[names[counter++]] is not long nanos) {
			return null;
		}

		var periodBuilder = new PeriodBuilder() {
			Years = years,
			Months = months,
			Weeks = weeks,
			Days = days,
			Nanoseconds = nanos,
		};

		return periodBuilder.Build();
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulnes")]
	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session) {
		if (value is Period val) {
			var counter = index;
			NHibernateUtil.Int16.NullSafeSet(cmd, val.Years, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, val.Months, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, val.Weeks, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, val.Days, counter++, session);
			NHibernateUtil.Int64.NullSafeSet(cmd, val.Nanoseconds, counter++, session);
		} else {
			var counter = index;
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int64.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property) {
		if (component is Period period) {
			return property switch {
				0 => period.Years,
				1 => period.Months,
				2 => period.Weeks,
				3 => period.Days,
				4 => period.Nanoseconds,
				_ => throw new ArgumentOutOfRangeException(nameof(property)),
			};
		} else {
			throw new UnexpectedTypeException<Period>(component);
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

		return ((Period)x).Equals((Period)y);
	}

	int ICompositeUserType.GetHashCode(object? x) {
		return x?.GetHashCode() ?? 0;
	}
}
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;
using static No1.NHibernateNodaTime.NodaTimeUtility;

namespace No1.NHibernateNodaTime;

public sealed class YearMonthCompositeUserType : ICompositeUserType
{
	internal static readonly string[] Columns = ["EraID", "CalendarID", "Year", "Month",];

	Type ICompositeUserType.ReturnedClass => typeof(YearMonth?);

	bool ICompositeUserType.IsMutable => false;

	string[] ICompositeUserType.PropertyNames => Columns;

	IType[] ICompositeUserType.PropertyTypes =>
	[
		NHibernateUtil.String,      // Era
		NHibernateUtil.String,      // Calendar
		NHibernateUtil.Int16,       // Year
		NHibernateUtil.Int16,       // Month
	];

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulnes")]
	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner) {
		var counter = 0;

		if (dr[names[counter++]] is not string eraId) {
			return null;
		}

		if (dr[names[counter++]] is not string calId) {
			return null;
		}

		if (dr[names[counter++]] is not short year) {
			return null;
		}

		if (dr[names[counter++]] is not short month) {
			return null;
		}

		var era = EraByID(eraId);
		var calendar = CalendarSystem.ForId(calId);

		return new YearMonth(era, year, month, calendar);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulnes")]
	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session) {
		if (value is YearMonth val) {
			var counter = index;
			NHibernateUtil.String.NullSafeSet(cmd, EraID(val.Era), counter++, session);
			NHibernateUtil.String.NullSafeSet(cmd, val.Calendar.Id, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, val.YearOfEra, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, val.Month, counter++, session);
		} else {
			var counter = index;
			NHibernateUtil.String.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.String.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property) {
		if (component is YearMonth val) {
			return property switch {
				0 => EraID(val.Era),
				1 => val.Calendar.Id,
				2 => val.YearOfEra,
				3 => val.Month,
				_ => throw new ArgumentOutOfRangeException(nameof(property)),
			};
		} else {
			throw new UnexpectedTypeException<YearMonth>(component);
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

		return ((YearMonth)x).Equals((YearMonth)y);
	}

	int ICompositeUserType.GetHashCode(object? x) {
		return x?.GetHashCode() ?? 0;
	}
}
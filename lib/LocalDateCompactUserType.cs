using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;
using static No1.NHibernateNodaTime.NodaTimeUtility;

namespace No1.NHibernateNodaTime;

public sealed class LocalDateCompactUserType : ICompositeUserType
{
	public static readonly ICompositeUserType Instance = new LocalDateCompactUserType();

	internal static readonly string[] Columns = ["Calendar", "Gregorian",];

	Type ICompositeUserType.ReturnedClass => typeof(LocalDate?);

	bool ICompositeUserType.IsMutable => false;

	string[] ICompositeUserType.PropertyNames => Columns;

	IType[] ICompositeUserType.PropertyTypes =>
	[
		NHibernateUtil.String,      // Calendar
		NHibernateUtil.Date,        // Date
	];

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulness")]
	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner) {
		var counter = 0;

		if (dr[names[counter++]] is not string calendarId) {
			return null;
		}

		if (dr[names[counter++]] is not DateTime dateTime) {
			return null;
		}

		var calendar = CalendarSystem.ForId(calendarId);
		return new LocalDate(dateTime.Year, dateTime.Month, dateTime.Day, calendar).WithCalendar(calendar);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulness")]
	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session) {
		if (value is LocalDate ld) {
			var counter = index;
			NHibernateUtil.String.NullSafeSet(cmd, ld.Calendar.Id, counter++, session);
			NHibernateUtil.Date.NullSafeSet(cmd, TryOrDefault(ld.ToDateTimeUnspecified), counter++, session);
		} else {
			var counter = index;
			NHibernateUtil.String.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Date.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property) {
		if (component is LocalDate ld) {
			return property switch {
				0 => ld.Calendar.Name,
				5 => TryOrDefault(ld.ToDateTimeUnspecified),
				_ => throw new ArgumentOutOfRangeException(nameof(property)),
			};
		} else {
			throw new UnexpectedTypeException<LocalDate>(component);
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

		return ((LocalDate)x).Equals((LocalDate)y);
	}

	int ICompositeUserType.GetHashCode(object? x) {
		return x?.GetHashCode() ?? 0;
	}
}
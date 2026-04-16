using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;
using NodaTime;
using NodaTime.Calendars;
using System.Data.Common;
using static No1.NHibernateNodaTime.NodaTimeUtility;

namespace No1.NHibernateNodaTime;

/// <summary>
/// </summary>
public sealed class LocalDateCompositeUserType : ICompositeUserType
{
	Type ICompositeUserType.ReturnedClass => typeof(LocalDate?);

	bool ICompositeUserType.IsMutable => false;

	internal static string[] Columns => ["Calendar", "Era", "Year", "Month", "Day", "Gregorian",];

	string[] ICompositeUserType.PropertyNames => Columns;

	IType[] ICompositeUserType.PropertyTypes =>
	[
		NHibernateUtil.String,		// Calendar
		NHibernateUtil.String,		// Era
		NHibernateUtil.Int16,		// Year
		NHibernateUtil.Int16,		// Month
		NHibernateUtil.Int16,		// Day
		NHibernateUtil.Date,		// Date
	];

	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
	{
		var counter = 0;

		if (dr[names[counter++]] is not string calendarId)
			return null;

		if (dr[names[counter++]] is not string eraId)
			return null;

		if (dr[names[counter++]] is not short year)
			return null;

		if (dr[names[counter++]] is not short month)
			return null;

		if (dr[names[counter++]] is not short day)
			return null;

		var calendar = CalendarSystem.ForId(calendarId);
		var era = EraByID(eraId);
		return new LocalDate(era, year, month, day, calendar);
	}

	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session)
	{
		if (value is LocalDate ld)
		{
			var counter = index;
			NHibernateUtil.String.NullSafeSet(cmd, ld.Calendar.Id, counter++, session);
			NHibernateUtil.String.NullSafeSet(cmd, EraID(ld.Era), counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, ld.YearOfEra, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, ld.Month, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, ld.Day, counter++, session);
			NHibernateUtil.Date.NullSafeSet(cmd, TryOrDefault(ld.ToDateTimeUnspecified), counter++, session);
		}
		else
		{
			var counter = index;
			NHibernateUtil.String.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.String.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Date.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property)
	{
		if (component is LocalDate ld)
		{
			return property switch
			{
				0 => ld.Calendar.Name,
				1 => EraID(ld.Era),
				2 => ld.YearOfEra,
				3 => ld.Month,
				4 => ld.Day,
				5 => TryOrDefault(ld.ToDateTimeUnspecified),
				_ => throw new ArgumentOutOfRangeException(nameof(property))
			};
		}
		else
		{
			throw new MismatchTypeException($"Object is not LocalDate, is {component?.GetType()?.Name ?? "NULL"}");
		}
	}

	void ICompositeUserType.SetPropertyValue(object component, int property, object value)
	{
		throw new InvalidOperationException("immutable");
	}

	object ICompositeUserType.DeepCopy(object value)
	{
		return value;
	}

	object ICompositeUserType.Disassemble(object value, ISessionImplementor session)
	{
		return value;
	}

	object ICompositeUserType.Assemble(object cached, ISessionImplementor session, object owner)
	{
		return cached;
	}

	object ICompositeUserType.Replace(object original, object target, ISessionImplementor session, object owner)
	{
		return original;
	}

	bool ICompositeUserType.Equals(object? x, object? y)
	{
		if (ReferenceEquals(x, y)) return true;
		if (x == null || y == null) return false;
		return ((LocalDate)x).Equals((LocalDate)y);
	}

	int ICompositeUserType.GetHashCode(object? x)
	{
		return x?.GetHashCode() ?? 0;
	}
}
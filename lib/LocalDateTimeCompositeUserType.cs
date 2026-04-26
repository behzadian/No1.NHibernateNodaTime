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
public sealed class LocalDateTimeCompositeUserType : ICompositeUserType
{
	Type ICompositeUserType.ReturnedClass => typeof(LocalDateTime?);

	bool ICompositeUserType.IsMutable => false;

	internal static string[] Columns => ["Calendar", "Year", "Month", "Day", "Nanos", "Gregorian",];

	string[] ICompositeUserType.PropertyNames => Columns;

	IType[] ICompositeUserType.PropertyTypes =>
	[
		NHibernateUtil.String,			// Calendar
		NHibernateUtil.Int16,			// Year
		NHibernateUtil.Int16,			// Month
		NHibernateUtil.Int16,			// Day
		NHibernateUtil.Int64,			// Time
		NHibernateUtil.Date,			// Date
	];

	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
	{
		var counter = 0;

		if (dr[names[counter++]] is not string calendarId)
			return null;

		if (dr[names[counter++]] is not short year)
			return null;

		if (dr[names[counter++]] is not short month)
			return null;

		if (dr[names[counter++]] is not short day)
			return null;

		if (dr[names[counter++]] is not long nanos)
			return null;

		var time = LocalTime.FromNanosecondsSinceMidnight(nanos);

		var calendar = CalendarSystem.ForId(calendarId);
		return new LocalDateTime(year, month, day, time.Hour, time.Minute, time.Second, time.Millisecond, calendar);
	}

	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session)
	{
		if (value is LocalDateTime ldt)
		{
			var counter = index;
			NHibernateUtil.String.NullSafeSet(cmd, ldt.Calendar.Id, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, ldt.YearOfEra, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, ldt.Month, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, ldt.Day, counter++, session);
			NHibernateUtil.Int64.NullSafeSet(cmd, ldt.TimeOfDay.NanosecondOfDay, counter++, session);
			NHibernateUtil.Date.NullSafeSet(cmd, TryOrDefault(ldt.ToDateTimeUnspecified), counter++, session);
		}
		else
		{
			var counter = index;
			NHibernateUtil.String.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int64.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Date.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property)
	{
		if (component is LocalDateTime ldt)
		{
			return property switch
			{
				0 => ldt.Calendar.Name,
				1 => EraID(ldt.Era),
				2 => ldt.YearOfEra,
				3 => ldt.Month,
				4 => ldt.Day,
				5 => ldt.TimeOfDay.NanosecondOfDay,
				6 => TryOrDefault(ldt.ToDateTimeUnspecified),
				_ => throw new ArgumentOutOfRangeException(nameof(property))
			};
		}
		else
		{
			throw new MismatchTypeException($"Object is not LocalDateTime, is {component?.GetType()?.Name ?? "NULL"}");
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
		return ((LocalDateTime)x).Equals((LocalDateTime)y);
	}

	int ICompositeUserType.GetHashCode(object? x)
	{
		return x?.GetHashCode() ?? 0;
	}
}
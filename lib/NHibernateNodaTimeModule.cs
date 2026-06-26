using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;

namespace No1.NHibernateNodaTime;

public static class NHibernateNodaTimeModule
{
	public static AutoPersistenceModel EnableNodaTime(this AutoPersistenceModel convention) {
		ArgumentNullException.ThrowIfNull(convention);
		return convention.Conventions.EnableNodaTime();
	}

	public static AutoPersistenceModel EnableNodaTime(this SetupConventionFinder<AutoPersistenceModel> convention) {
		ArgumentNullException.ThrowIfNull(convention);
		return convention.Add<NodaTimeTypesComponentConvention>();
	}

	internal static void MapInstantCompactProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<InstantCompactUserType>(propertyPart, columnNameBuilder, propertyName, InstantCompactUserType.Columns);
	}

	internal static void MapInstantCompleteProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<InstantCompleteUserType>(propertyPart, columnNameBuilder, propertyName, InstantCompleteUserType.Columns);
	}

	internal static void MapZonedDateTimeCompactProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<ZonedDateTimeCompactUserType>(propertyPart, columnNameBuilder, propertyName, ZonedDateTimeCompactUserType.Columns);
	}

	internal static void MapZonedDateTimeCompleteProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<ZonedDateTimeCompleteUserType>(propertyPart, columnNameBuilder, propertyName, ZonedDateTimeCompleteUserType.Columns);
	}

	internal static void MapDurationCompactProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<DurationCompactUserType>(propertyPart, columnNameBuilder, propertyName);
	}

	internal static void MapDurationCompleteProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<DurationCompleteUserType>(propertyPart, columnNameBuilder, propertyName, DurationCompleteUserType.Columns);
	}

	internal static void MapAnnualDateProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<AnnualDateCompositeUserType>(propertyPart, columnNameBuilder, propertyName, AnnualDateCompositeUserType.Columns);
	}

	internal static void MapLocalDateCompactProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<LocalDateCompactUserType>(propertyPart, columnNameBuilder, propertyName, LocalDateCompactUserType.Columns);
	}

	internal static void MapLocalDateCompleteProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<LocalDateCompleteUserType>(propertyPart, columnNameBuilder, propertyName, LocalDateCompleteUserType.Columns);
	}

	internal static void MapLocalDateTimeCompactProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<LocalDateTimeCompactUserType>(propertyPart, columnNameBuilder, propertyName, LocalDateTimeCompactUserType.Columns);
	}

	internal static void MapLocalDateTimeCompleteProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<LocalDateTimeCompleteUserType>(propertyPart, columnNameBuilder, propertyName, LocalDateTimeCompleteUserType.Columns);
	}

	internal static void MapOffsetDateCompactProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<OffsetDateCompactUserType>(propertyPart, columnNameBuilder, propertyName, OffsetDateCompactUserType.Columns);
	}

	internal static void MapOffsetDateCompleteProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<OffsetDateCompleteUserType>(propertyPart, columnNameBuilder, propertyName, OffsetDateCompleteUserType.Columns);
	}

	internal static void MapOffsetDateTimeCompactProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<OffsetDateTimeCompactUserType>(propertyPart, columnNameBuilder, propertyName, OffsetDateTimeCompactUserType.Columns);
	}

	internal static void MapOffsetDateTimeCompleteProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<OffsetDateTimeCompleteUserType>(propertyPart, columnNameBuilder, propertyName, OffsetDateTimeCompleteUserType.Columns);
	}

	internal static void MapOffsetTimeProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<OffsetTimeCompositeUserType>(propertyPart, columnNameBuilder, propertyName, OffsetTimeCompositeUserType.Columns);
	}

	internal static void MapYearMonthProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<YearMonthCompositeUserType>(propertyPart, columnNameBuilder, propertyName, YearMonthCompositeUserType.Columns);
	}

	internal static void MapPeriodCompactProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<PeriodCompactUserType>(propertyPart, columnNameBuilder, propertyName);
	}

	internal static void MapPeriodCompleteProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<PeriodCompleteUserType>(propertyPart, columnNameBuilder, propertyName, PeriodCompleteUserType.Columns);
	}

	internal static void MapLocalTimeProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<LocalTimeUserType>(propertyPart);
	}

	internal static void MapOffsetProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<OffsetUserType>(propertyPart);
	}

	private static void MapColumns<T>(PropertyPart propertyPart, Func<string, string>? columnNameBuilder = null, string? prefix = null, params string[] columns) {
		propertyPart.CustomType<T>();
		propertyPart.Columns.Clear();
		if (prefix.IsUsable() && columns?.Length > 1) {
			foreach (var property in columns) {
				var propertyFullName = prefix + property;
				var columnName = columnNameBuilder?.Invoke(propertyFullName) ?? propertyFullName;
				propertyPart.Columns.Add(columnName);
			}
		}
	}
}
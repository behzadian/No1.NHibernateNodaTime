using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;
using NodaTime;

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

	internal static void MapInstantProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<InstantCompositeUserType>(propertyPart, columnNameBuilder, propertyName, InstantCompositeUserType.Columns);
	}

	internal static void MapZonedDateTimeProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<ZonedDateTimeCompositeUserType>(propertyPart, columnNameBuilder, propertyName, ZonedDateTimeCompositeUserType.Columns);
	}

	internal static void MapDurationProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<DurationCompositeUserType>(propertyPart, columnNameBuilder, propertyName, DurationCompositeUserType.Columns);
	}

	internal static void MapAnnualDateProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<AnnualDateCompositeUserType>(propertyPart, columnNameBuilder, propertyName, AnnualDateCompositeUserType.Columns);
	}

	internal static void MapLocalDateProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<LocalDateCompositeUserType>(propertyPart, columnNameBuilder, propertyName, LocalDateCompositeUserType.Columns);
	}

	internal static void MapLocalDateTimeProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<LocalDateTimeCompositeUserType>(propertyPart, columnNameBuilder, propertyName, LocalDateTimeCompositeUserType.Columns);
	}

	internal static void MapOffsetDateProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<OffsetDateCompositeUserType>(propertyPart, columnNameBuilder, propertyName, OffsetDateCompositeUserType.Columns);
	}

	internal static void MapOffsetDateTimeProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<OffsetDateTimeCompositeUserType>(propertyPart, columnNameBuilder, propertyName, OffsetDateTimeCompositeUserType.Columns);
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

	internal static void MapPeriodProperty(PropertyPart propertyPart, string propertyName, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(propertyPart);
		ArgumentNullException.ThrowIfNull(propertyName);
		MapColumns<PeriodCompositeUserType>(propertyPart, columnNameBuilder, propertyName, PeriodCompositeUserType.Columns);
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
		if (prefix.IsUsable() && columns?.Length > 0) {
			foreach (var property in columns) {
				var propertyFullName = $"{prefix}{property}";
				var columnName = columnNameBuilder?.Invoke(propertyFullName) ?? propertyFullName;
				propertyPart.Columns.Add(columnName);
			}
		}
	}
}
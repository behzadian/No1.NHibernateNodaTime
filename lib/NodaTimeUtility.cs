using FluentNHibernate.Automapping;
using NodaTime;
using NodaTime.Calendars;
using System.Reflection;
using System.Text.RegularExpressions;
using static No1.NHibernateNodaTime.NHibernateNodaTimeModule;

namespace No1.NHibernateNodaTime;

public static partial class NodaTimeUtility
{
	private static readonly Dictionary<string, Era> Eras = new() {
		{ Era.AnnoHegirae.Name, Era.AnnoHegirae },
		{ "Martyrum", Era.AnnoMartyrum },
		{ "Mundi", Era.AnnoMundi },
		{ Era.AnnoPersico.Name, Era.AnnoPersico },
		{ Era.Bahai.Name, Era.Bahai },
		{ Era.BeforeCommon.Name, Era.BeforeCommon },
		{ Era.Common.Name, Era.Common },
	};

	public static void OverrideEntity<TEntity>(AutoMapping<TEntity> mapping, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(mapping);

		foreach (var property in typeof(TEntity).GetProperties()) {
			switch (property) {
				case PropertyInfo when property.PropertyType.Is<AnnualDate>():
					MapAnnualDateProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<Duration>() && StorageMethodAttribute.CompactStorageEnabled(property):
					MapDurationCompactProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<Duration>() && StorageMethodAttribute.CompleteStorageEnabled(property):
					MapDurationCompleteProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<Instant>() && StorageMethodAttribute.CompactStorageEnabled(property):
					MapInstantCompactProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<Instant>() && StorageMethodAttribute.CompleteStorageEnabled(property):
					MapInstantCompleteProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<LocalDate>() && StorageMethodAttribute.CompactStorageEnabled(property):
					MapLocalDateCompactProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<LocalDate>() && StorageMethodAttribute.CompleteStorageEnabled(property):
					MapLocalDateCompleteProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<LocalDateTime>() && StorageMethodAttribute.CompactStorageEnabled(property):
					MapLocalDateTimeCompactProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<LocalDateTime>() && StorageMethodAttribute.CompleteStorageEnabled(property):
					MapLocalDateTimeCompleteProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<LocalTime>():
					MapLocalTimeProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<OffsetDate>() && StorageMethodAttribute.CompactStorageEnabled(property):
					MapOffsetDateCompactProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<OffsetDate>() && StorageMethodAttribute.CompleteStorageEnabled(property):
					MapOffsetDateCompleteProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<OffsetDateTime>() && StorageMethodAttribute.CompactStorageEnabled(property):
					MapOffsetDateTimeCompactProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<OffsetDateTime>() && StorageMethodAttribute.CompleteStorageEnabled(property):
					MapOffsetDateTimeCompleteProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<OffsetTime>():
					MapOffsetTimeProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<Offset>():
					MapOffsetProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType == typeof(Period) && StorageMethodAttribute.CompactStorageEnabled(property):
					MapPeriodCompactProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType == typeof(Period) && StorageMethodAttribute.CompleteStorageEnabled(property):
					MapPeriodCompleteProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<YearMonth>():
					MapYearMonthProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<ZonedDateTime>() && StorageMethodAttribute.CompactStorageEnabled(property):
					MapZonedDateTimeCompactProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<ZonedDateTime>() && StorageMethodAttribute.CompleteStorageEnabled(property):
					MapZonedDateTimeCompleteProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;
			}
		}
	}

	internal static bool IsUsable(this string? text) {
		return !string.IsNullOrEmpty(text);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "It's safe to suppress a warning when you're not making security decisions based on the result of the normalization (for example, when you're displaying the result in the UI).")]
	internal static string SnakeCase(this string name) {
		return WordPattern().Replace(name, "$1_$2").ToLowerInvariant();
	}

	internal static int OnlyNanoseconds(this Instant instant) {
		return instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds;
	}

	internal static bool Is<T>(this Type type)
		where T : struct {
		if (type == typeof(T)) {
			return true;
		}

		return type == typeof(T?);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Skip")]
	internal static T? TryOrDefault<T>(Func<T> func) {
		try {
			return func();
		} catch (Exception) {
			return default;
		}
	}

	internal static Era EraByID(string eraId) {
		return Eras[eraId] ?? throw new UnsupportedValueException(eraId);
	}

	internal static string EraID(Era era) {
		return Eras.FirstOrDefault(x => x.Value.Equals(era)).Key ?? throw new UnsupportedValueException(era);
	}

	[GeneratedRegex(@"([a-z\d])([A-Z])")]
	private static partial Regex WordPattern();
}
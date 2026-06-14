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

	internal static void OverrideEntity<TEntity>(AutoMapping<TEntity> mapping, Func<string, string>? columnNameBuilder = null) {
		ArgumentNullException.ThrowIfNull(mapping);

		foreach (var property in typeof(TEntity).GetProperties()) {
			switch (property) {
				case PropertyInfo when property.PropertyType.Is<AnnualDate>():
					MapAnnualDateProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<Duration>():
					MapDurationProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<Instant>():
					MapInstantProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<LocalDate>():
					MapLocalDateProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<LocalDateTime>():
					MapLocalDateTimeProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<LocalTime>():
					MapLocalTimeProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<OffsetDate>():
					MapOffsetDateProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<OffsetDateTime>():
					MapOffsetDateTimeProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<OffsetTime>():
					MapOffsetTimeProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<Offset>():
					MapOffsetProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType == typeof(Period):
					MapPeriodProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<YearMonth>():
					MapYearMonthProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;

				case PropertyInfo when property.PropertyType.Is<ZonedDateTime>():
					MapZonedDateTimeProperty(mapping.Map(ReflectionUtility.GetPropertExpression<TEntity>(property.Name)), property.Name, columnNameBuilder);
					break;
			}
		}
	}

	[GeneratedRegex(@"([a-z\d])([A-Z])")]
	private static partial Regex WordPattern();
}
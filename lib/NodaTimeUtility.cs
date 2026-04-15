using FluentNHibernate.Mapping;
using NHibernate.Mapping;
using NodaTime;
using NodaTime.Calendars;

namespace No1.NHibernateNodaTime;

public static class NodaTimeUtility
{
	private static long dateTimeMinTicks = DateTime.MinValue.Ticks;
	private static long dateTimeMaxTicks = DateTime.MaxValue.Ticks;

	public static int OnlyNanoseconds(this Instant instant)
	{
		return instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds;
	}

	internal static DateTime AsUtc(DateTime utc)
	{
		if (utc.Kind == DateTimeKind.Utc)
		{
			return utc;
		}
		return new DateTime(utc.Ticks, DateTimeKind.Utc);
	}

	internal static DateTime? ToDateTimeUtcOrNull(this Instant instant)
	{
		var ticks = instant.ToUnixTimeTicks();
		if (ticks < dateTimeMinTicks)
			return null;
		if (dateTimeMaxTicks < ticks)
			return null;
		try
		{
			return instant.ToDateTimeUtc();
		}
		catch (Exception)
		{
			return null;
		}
	}

	internal static DateTime? ToDateTimeUtcOrNull(this ZonedDateTime zdt)
	{
		try
		{
			return zdt.ToDateTimeUtc();
		}
		catch (Exception)
		{
			return null;
		}
	}

	internal static DateTime? ToDateTimeUnspecifiedOrNull(this ZonedDateTime zdt)
	{
		try
		{
			return zdt.ToDateTimeUnspecified();
		}
		catch (Exception)
		{
			return null;
		}
	}

	internal static DateTime? ToDateTimeUnspecifiedOrNull(this LocalDate value)
	{
		try
		{
			return value.ToDateTimeUnspecified();
		}
		catch (Exception)
		{
			return null;
		}
	}


	private static readonly Dictionary<string, Era> Eras = new() {
			{   Era.AnnoHegirae.Name    , Era.AnnoHegirae},
			{   /*Era.AnnoMartyrum*/ "Martyrum"    , Era.AnnoMartyrum},
			{   /*Era.AnnoMundi*/ "Mundi" , Era.AnnoMundi},
			{   Era.AnnoPersico.Name    , Era.AnnoPersico},
			{   Era.Bahai.Name    , Era.Bahai},
			{   Era.BeforeCommon.Name    , Era.BeforeCommon},
			{   Era.Common.Name    , Era.Common},
		};

	internal static Era GetEra(string eraId)
	{
		return Eras[eraId] ?? throw new Exception($"Unable to find Era with name {eraId}");
	}

	internal static string EraName(Era era)
	{
		return Eras.FirstOrDefault(x => x.Value.Equals(era)).Key ?? throw new Exception("Era not found");
	}
}
using FluentNHibernate.Mapping;
using NHibernate.Mapping;
using NodaTime;
using NodaTime.Calendars;

namespace No1.NHibernateNodaTime;

public static class NodaTimeUtility
{

	public static int OnlyNanoseconds(this Instant instant)
	{
		return instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds;
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
	internal static T? TryOrDefault<T>(Func<T> func)
	{
		try
		{
			return func();
		}
		catch (Exception)
		{
			return default;
		}
	}


	private static readonly Dictionary<string, Era> Eras = new() {
		{ Era.AnnoHegirae.Name, Era.AnnoHegirae},
		{ "Martyrum", Era.AnnoMartyrum},
		{ "Mundi", Era.AnnoMundi},
		{ Era.AnnoPersico.Name, Era.AnnoPersico},
		{ Era.Bahai.Name, Era.Bahai},
		{ Era.BeforeCommon.Name, Era.BeforeCommon},
		{ Era.Common.Name, Era.Common},
	};

	internal static Era EraByID(string eraId)
	{
		return Eras[eraId] ?? throw new MismatchTypeException($"Unable to find Era with name {eraId}");
	}

	internal static string EraID(Era era)
	{
		return Eras.FirstOrDefault(x => x.Value.Equals(era)).Key ?? throw new MismatchTypeException("Era not found");
	}
}
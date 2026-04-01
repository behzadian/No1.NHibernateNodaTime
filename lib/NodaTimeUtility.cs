using NodaTime;

namespace No1.NHibernateNodaTime;

public static class NodaTimeUtility
{
	public static int Nanoseconds(this Instant instant)
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
}
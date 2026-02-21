using NodaTime;

namespace No1.NHibernateNodaTimeTests;

public static class NodaTimeTestUtility
{
    public static int Nanoseconds(this Instant instant)
    {
        return instant.ToUnixTimeSecondsAndNanoseconds().nanoseconds;
    }
}
using NodaTime;

namespace No1.NHibernateNodaTimeTests.TestEntities;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class ZonedDateTimeEntity
{
	public virtual int Id { get; set; }
	public virtual string Name { get; set; } = string.Empty;
	public virtual ZonedDateTime ZdtValauable { get; set; } = SystemClock.Instance.GetCurrentInstant().InUtc();
	public virtual ZonedDateTime? ZdtNullable { get; set; }
}

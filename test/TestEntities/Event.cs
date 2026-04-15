using NodaTime;

namespace No1.NHibernateNodaTimeTests.TestEntities;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class Event
{
	public virtual int Id { get; set; }
	public virtual string Name { get; set; } = string.Empty;
	public virtual Instant InstantValuable { get; set; }
	public virtual Instant? InstantNullable { get; set; }
	public virtual ZonedDateTime ZdtValauable { get; set; } = SystemClock.Instance.GetCurrentInstant().InUtc();
	public virtual ZonedDateTime? ZdtNullable { get; set; }
	public virtual Duration DurationValauable { get; set; } = Duration.Epsilon;
	public virtual Duration? DurationNullable { get; set; }
	public virtual AnnualDate AnnualDateValauable { get; set; }
	public virtual AnnualDate? AnnualDateNullable { get; set; }
	public virtual LocalDate LdValauable { get; set; } = new LocalDate();
	public virtual LocalDate? LdNullable { get; set; }
}

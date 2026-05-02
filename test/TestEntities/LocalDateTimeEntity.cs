using NodaTime;

namespace No1.NHibernateNodaTimeTests.TestEntities;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class LocalDateTimeEntity
{
	public virtual int Id { get; set; }
	public virtual string Name { get; set; } = string.Empty;
	public virtual LocalDateTime LdtValauable { get; set; } = new LocalDateTime();
	public virtual LocalDateTime? LdtNullable { get; set; }
}

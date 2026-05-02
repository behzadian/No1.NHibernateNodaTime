using NodaTime;

namespace No1.NHibernateNodaTimeTests.TestEntities;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class LocalTimeEntity
{
	public virtual int Id { get; set; }
	public virtual string Name { get; set; } = string.Empty;
	public virtual LocalTime LtValauable { get; set; }
	public virtual LocalTime? LtNullable { get; set; }
}

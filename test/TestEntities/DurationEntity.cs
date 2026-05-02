using NodaTime;

namespace No1.NHibernateNodaTimeTests.TestEntities;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class DurationEntity
{
	public virtual int Id { get; set; }
	public virtual string Name { get; set; } = string.Empty;
	public virtual Duration DurationValauable { get; set; } = Duration.Epsilon;
	public virtual Duration? DurationNullable { get; set; }
}

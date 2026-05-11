using NodaTime;

namespace No1.NHibernateNodaTimeTests.TestEntities;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class OffsetDateEntity
{
	public virtual int Id { get; set; }
	public virtual OffsetDate Valauable { get; set; }
	public virtual OffsetDate? Nullable { get; set; }
}
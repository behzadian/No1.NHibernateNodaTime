using NodaTime;

namespace No1.NHibernateNodaTimeTests.TestEntities;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class OffsetDateTimeEntity
{
	public virtual int Id { get; set; }
	public virtual OffsetDateTime Valauable { get; set; }
	public virtual OffsetDateTime? Nullable { get; set; }
}

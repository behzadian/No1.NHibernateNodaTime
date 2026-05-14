using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class OffsetTimeEntity
{
	public virtual int Id { get; set; }
	public virtual OffsetTime Valauable { get; set; }
	public virtual OffsetTime? Nullable { get; set; }
}
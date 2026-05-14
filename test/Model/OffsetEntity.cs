using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class OffsetEntity
{
	public virtual int Id { get; set; }
	public virtual Offset Valauable { get; set; }
	public virtual Offset? Nullable { get; set; }
}
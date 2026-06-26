using No1.NHibernateNodaTime;
using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
[StorageMethod(StorageMethods.Compact)]
public class OffsetDateTimeCompactEntity
{
	public virtual int Id { get; set; }
	public virtual OffsetDateTime Valauable { get; set; }
	public virtual OffsetDateTime? Nullable { get; set; }
}
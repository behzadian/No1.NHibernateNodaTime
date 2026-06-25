using No1.NHibernateNodaTime;
using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
[StorageMethod(StorageMethods.Complete)]
public class OffsetDateEntity
{
	public virtual int Id { get; set; }
	public virtual OffsetDate Valauable { get; set; }
	public virtual OffsetDate? Nullable { get; set; }
}
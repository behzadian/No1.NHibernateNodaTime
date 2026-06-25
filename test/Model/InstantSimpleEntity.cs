using No1.NHibernateNodaTime;
using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class InstantCompactEntity
{
	public virtual int Id { get; set; }

	[StorageMethod(StorageMethods.Compact)]
	public virtual Instant Valuable { get; set; }

	[StorageMethod(StorageMethods.Compact)]
	public virtual Instant? Nullable { get; set; }
}
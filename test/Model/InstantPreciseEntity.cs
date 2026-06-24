using No1.NHibernateNodaTime;
using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class InstantPreciseEntity
{
	public virtual int Id { get; set; }
	public virtual string Name { get; set; } = string.Empty;

	[StorageMethod(StorageMethods.Precise)]
	public virtual Instant Valuable { get; set; }
	
	[StorageMethod(StorageMethods.Precise)]
	public virtual Instant? Nullable { get; set; }
}
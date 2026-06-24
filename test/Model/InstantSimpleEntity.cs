using No1.NHibernateNodaTime;
using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class InstantSimpleEntity
{
	public virtual int Id { get; set; }
	public virtual string Name { get; set; } = string.Empty;

	[StorageMethod(StorageMethods.Simple)]
	public virtual Instant Valuable { get; set; }

	[StorageMethod(StorageMethods.Simple)]
	public virtual Instant? Nullable { get; set; }
}
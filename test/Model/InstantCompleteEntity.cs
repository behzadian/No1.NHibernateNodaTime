using No1.NHibernateNodaTime;
using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
[StorageMethod(StorageMethods.Complete)]
public class InstantCompleteEntity
{
	public virtual int Id { get; set; }
	public virtual string Name { get; set; } = string.Empty;

	public virtual Instant Valuable { get; set; }

	public virtual Instant? Nullable { get; set; }
}
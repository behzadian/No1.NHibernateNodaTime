using No1.NHibernateNodaTime;
using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
[StorageMethod(StorageMethods.Compact)]
public class DurationCompactEntity
{
	public virtual int Id { get; set; }
	public virtual Duration Valauable { get; set; } = Duration.Epsilon;
	public virtual Duration? Nullable { get; set; }
}
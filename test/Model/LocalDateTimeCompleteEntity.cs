using No1.NHibernateNodaTime;
using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
[StorageMethod(StorageMethods.Complete)]
public class LocalDateTimeCompleteEntity
{
	public virtual int Id { get; set; }
	public virtual LocalDateTime Valauable { get; set; } = new LocalDateTime();
	public virtual LocalDateTime? Nullable { get; set; }
}
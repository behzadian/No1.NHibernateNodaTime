using No1.NHibernateNodaTime;
using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
[StorageMethod(StorageMethods.Complete)]
public class YearMonthEntity
{
	public virtual int Id { get; set; }
	public virtual YearMonth Valauable { get; set; }
	public virtual YearMonth? Nullable { get; set; }
}
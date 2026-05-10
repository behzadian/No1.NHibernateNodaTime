using NodaTime;

namespace No1.NHibernateNodaTimeTests.TestEntities;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class YearMonthEntity
{
	public virtual int Id { get; set; }
	public virtual YearMonth Valauable { get; set; }
	public virtual YearMonth? Nullable { get; set; }
}

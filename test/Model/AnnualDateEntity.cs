using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class AnnualDateEntity
{
	public virtual int Id { get; set; }
	public virtual AnnualDate Valauable { get; set; }
	public virtual AnnualDate? Nullable { get; set; }
}
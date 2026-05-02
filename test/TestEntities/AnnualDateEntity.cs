using NodaTime;

namespace No1.NHibernateNodaTimeTests.TestEntities;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class AnnualDateEntity
{
	public virtual int Id { get; set; }
	public virtual string Name { get; set; } = string.Empty;
	public virtual AnnualDate AnnualDateValauable { get; set; }
	public virtual AnnualDate? AnnualDateNullable { get; set; }
}

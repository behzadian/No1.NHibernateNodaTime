using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class LocalDateEntity
{
	public virtual int Id { get; set; }
	public virtual string Name { get; set; } = string.Empty;
	public virtual LocalDate Valauable { get; set; } = new LocalDate();
	public virtual LocalDate? Nullable { get; set; }
}
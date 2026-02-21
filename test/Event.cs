using NodaTime;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Sample entity for testing NodaTime Instant persistence
/// </summary>
public class Event
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; } = string.Empty;
    public virtual Instant CreatedAt { get; set; }
    public virtual Instant? ModifiedAt { get; set; }
    public virtual Instant EventDate { get; set; }

    // For NHibernate
    protected Event() { }

    public Event(string name, Instant createdAt, Instant eventDate)
    {
        Name = name;
        CreatedAt = createdAt;
        EventDate = eventDate;
    }

    public virtual void UpdateModifiedAt(Instant modifiedAt)
    {
        ModifiedAt = modifiedAt;
    }
}

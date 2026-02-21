using FluentNHibernate.Mapping;

namespace No1.NHibernateNodaTimeTests;

public class EventMap : ClassMap<Event>
{
    public EventMap()
    {
        Table("events");

        Id(x => x.Id)
            .GeneratedBy.Identity()
            .Column("id");

        Map(x => x.Name)
            .Column("name")
            .Length(200)
            .Not.Nullable();

        Map(x => x.CreatedAt)
            .Column("created_at_seconds")
            .Column("created_at_nanoseconds")
            .Not.Nullable();

        Map(x => x.ModifiedAt)
            .Column("modified_at_seconds")
            .Column("modified_at_nanoseconds")
            .Nullable();

        Map(x => x.EventDate)
            .Column("event_date_seconds")
            .Column("event_date_nanoseconds")
            .Not.Nullable();
    }
}

using FluentNHibernate.Mapping;
using No1.NodaTimeNHibernate.Tests.TestEntities;

namespace No1.NodaTimeNHibernate.Tests.Mappings;

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
            .Column("created_at")
            .Not.Nullable();
        
        Map(x => x.ModifiedAt)
            .Column("modified_at")
            .Nullable();
        
        Map(x => x.EventDate)
            .Column("event_date")
            .Not.Nullable();
    }
}

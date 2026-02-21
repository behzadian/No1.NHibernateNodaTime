using FluentNHibernate.Automapping;
using NodaTime;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// AutoMapping configuration for test entities
/// </summary>
public class AutoMappingConfiguration : DefaultAutomappingConfiguration
{
    public override bool ShouldMap(Type type)
    {
        // Only map classes in TestEntities namespace
        return type.Namespace != null && 
               type.Namespace.Contains("TestEntities");
    }

    public override bool IsId(FluentNHibernate.Member member)
    {
        // Map properties named "Id" as identifiers
        return member.Name == "Id";
    }
    public override bool IsComponent(Type type)
    {
        // Treat Instant as a component (value type), not an entity
        return type == typeof(Instant);// || type == typeof(Instant?);
    }
}

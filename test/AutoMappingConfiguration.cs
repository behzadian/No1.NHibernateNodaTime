using FluentNHibernate;
using FluentNHibernate.Automapping;
using NHibernate.Criterion;
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
    
    public override bool ShouldMap(Member member)
    {
        var type = member.PropertyType;
        if (type == typeof(Instant) || type == typeof(Instant?))
            return true;
        return base.ShouldMap(member);
    }
}

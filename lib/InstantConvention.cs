using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NodaTime;

namespace No1.NHibernateNodaTime;

/// <summary>
/// Convention to automatically apply InstantCompositeUserType to Instant properties
/// and set column prefix based on property name
/// </summary>
public class InstantCompositeConvention : IPropertyConvention
{
    public void Apply(IPropertyInstance instance)
    {
        if (instance.Property.PropertyType == typeof(Instant) ||
            instance.Property.PropertyType == typeof(Instant?))
        {
            // Use property name as column prefix (e.g., CreatedAt -> created_at_)
            var columnPrefix = ToSnakeCase(instance.Property.Name) + "_";
            
            instance.CustomType<InstantCompositeUserType>();
            instance.Access.Property();
            
            // Note: ColumnPrefix needs to be set in the mapping, not via convention
            // This convention just sets the custom type
        }
    }

    private static string ToSnakeCase(string name)
    {
        return string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
            .ToLower();
    }
}
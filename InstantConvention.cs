using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NodaTime;

namespace NHibernate.NodaTime;

public class InstantConvention : IPropertyConvention, IPropertyConventionAcceptance
{
    public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
    {
        criteria.Expect(x => x.Property.PropertyType == typeof(Instant) ||
                            x.Property.PropertyType == typeof(Instant?));
    }

    public void Apply(IPropertyInstance instance)
    {
        instance.CustomType<InstantType>();
    }
}

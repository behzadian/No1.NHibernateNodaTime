using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Type;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.NodaTime;

public class LocalDateTimeConvention : IPropertyConvention, IPropertyConventionAcceptance
{
    public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
    {
        criteria.Expect(x => x.Property.PropertyType == typeof(LocalDateTime) ||
                            x.Property.PropertyType == typeof(LocalDateTime?));
    }

    public void Apply(IPropertyInstance instance)
    {
        instance.CustomType<LocalDateTimeType>();
    }
}

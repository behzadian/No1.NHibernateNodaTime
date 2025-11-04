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

public class LocalDateConvention : IPropertyConvention, IPropertyConventionAcceptance
{
    public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
    {
        criteria.Expect(x => x.Property.PropertyType == typeof(LocalDate) ||
                            x.Property.PropertyType == typeof(LocalDate?));
    }

    public void Apply(IPropertyInstance instance)
    {
        instance.CustomType<LocalDateType>();
    }
}

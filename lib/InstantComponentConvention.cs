using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NHibernate;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace No1.NHibernateNodaTime;

public class InstantComponentConvention : IPropertyConvention
{
    void IConvention<IPropertyInspector, IPropertyInstance>.Apply(IPropertyInstance instance)
    {
		switch (instance.Type) 
		{
			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(Instant) || instance.Type == typeof(Instant?):
				instance.CustomType<InstantCompositeUserType>(instance.Name + "_");
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(ZonedDateTime) || instance.Type == typeof(ZonedDateTime?):
				instance.CustomType<ZonedDateTimeCompositeUserType>(instance.Name + "_");
				break;
		}
    }
}

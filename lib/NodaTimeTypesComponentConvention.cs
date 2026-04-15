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

public class NodaTimeTypesComponentConvention : IPropertyConvention
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

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(Duration) || instance.Type == typeof(Duration?):
				instance.CustomType<DurationCompositeUserType>(instance.Name + "_");
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(AnnualDate) || instance.Type == typeof(AnnualDate?):
				instance.CustomType<AnnualDateCompositeUserType>(instance.Name + "_");
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(LocalDate) || instance.Type == typeof(LocalDate?):
				instance.CustomType<LocalDateCompositeUserType>(instance.Name + "_");
				break;
		}
    }
}

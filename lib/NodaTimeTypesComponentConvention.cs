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

public sealed class NodaTimeTypesComponentConvention : IPropertyConvention
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

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(LocalDateTime) || instance.Type == typeof(LocalDateTime?):
				instance.CustomType<LocalDateTimeCompositeUserType>(instance.Name + "_");
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(LocalTime) || instance.Type == typeof(LocalTime?):
				instance.CustomType<LocalTimeUserType>();
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(Offset) || instance.Type == typeof(Offset?):
				instance.CustomType<OffsetUserType>();
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(OffsetDate) || instance.Type == typeof(OffsetDate?):
				instance.CustomType<OffsetDateCompositeUserType>();
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(OffsetDateTime) || instance.Type == typeof(OffsetDateTime?):
				instance.CustomType<OffsetDateTimeCompositeUserType>();
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(OffsetTime) || instance.Type == typeof(OffsetTime?):
				instance.CustomType<OffsetTimeCompositeUserType>();
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(YearMonth) || instance.Type == typeof(YearMonth?):
				instance.CustomType<YearMonthCompositeUserType>();
				break;
		}
	}
}
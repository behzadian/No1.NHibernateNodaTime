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
		var separator = "";
		switch (instance.Type)
		{
			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(Instant) || instance.Type == typeof(Instant?):
				instance.CustomType<InstantCompositeUserType>(instance.Name + separator);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(ZonedDateTime) || instance.Type == typeof(ZonedDateTime?):
				instance.CustomType<ZonedDateTimeCompositeUserType>(instance.Name + separator);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(Duration) || instance.Type == typeof(Duration?):
				instance.CustomType<DurationCompositeUserType>(instance.Name + separator);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(AnnualDate) || instance.Type == typeof(AnnualDate?):
				instance.CustomType<AnnualDateCompositeUserType>(instance.Name + separator);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(LocalDate) || instance.Type == typeof(LocalDate?):
				instance.CustomType<LocalDateCompositeUserType>(instance.Name + separator);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(LocalDateTime) || instance.Type == typeof(LocalDateTime?):
				instance.CustomType<LocalDateTimeCompositeUserType>(instance.Name + separator);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(LocalTime) || instance.Type == typeof(LocalTime?):
				instance.CustomType<LocalTimeUserType>(instance.Name + separator);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(Offset) || instance.Type == typeof(Offset?):
				instance.CustomType<OffsetUserType>(instance.Name + separator);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(OffsetDate) || instance.Type == typeof(OffsetDate?):
				instance.CustomType<OffsetDateCompositeUserType>(instance.Name + separator);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(OffsetDateTime) || instance.Type == typeof(OffsetDateTime?):
				instance.CustomType<OffsetDateTimeCompositeUserType>(instance.Name + separator);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(OffsetTime) || instance.Type == typeof(OffsetTime?):
				instance.CustomType<OffsetTimeCompositeUserType>(instance.Name + separator);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(YearMonth) || instance.Type == typeof(YearMonth?):
				instance.CustomType<YearMonthCompositeUserType>(instance.Name + separator);
				break;
		}
	}
}
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NodaTime;

namespace No1.NHibernateNodaTime;

public sealed class NodaTimeTypesComponentConvention : IPropertyConvention
{
	void IConvention<IPropertyInspector, IPropertyInstance>.Apply(IPropertyInstance instance) {
		var prefix = instance.Name.SnakeCase();

		switch (instance.Type) {
			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(Instant) || instance.Type == typeof(Instant?):
				instance.CustomType<InstantCompactUserType>(prefix);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(ZonedDateTime) || instance.Type == typeof(ZonedDateTime?):
				instance.CustomType<ZonedDateTimeCompleteUserType>(prefix);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(Duration) || instance.Type == typeof(Duration?):
				instance.CustomType<DurationCompleteUserType>(prefix);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(AnnualDate) || instance.Type == typeof(AnnualDate?):
				instance.CustomType<AnnualDateCompositeUserType>(prefix);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(LocalDate) || instance.Type == typeof(LocalDate?):
				instance.CustomType<LocalDateCompleteUserType>(prefix);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(LocalDateTime) || instance.Type == typeof(LocalDateTime?):
				instance.CustomType<LocalDateTimeCompleteUserType>(prefix);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(LocalTime) || instance.Type == typeof(LocalTime?):
				instance.CustomType<LocalTimeUserType>(prefix);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(Offset) || instance.Type == typeof(Offset?):
				instance.CustomType<OffsetUserType>(prefix);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(OffsetDate) || instance.Type == typeof(OffsetDate?):
				instance.CustomType<OffsetDateCompleteUserType>(prefix);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(OffsetDateTime) || instance.Type == typeof(OffsetDateTime?):
				instance.CustomType<OffsetDateTimeCompleteUserType>(prefix);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(OffsetTime) || instance.Type == typeof(OffsetTime?):
				instance.CustomType<OffsetTimeCompositeUserType>(prefix);
				break;

			case FluentNHibernate.MappingModel.TypeReference when instance.Type == typeof(YearMonth) || instance.Type == typeof(YearMonth?):
				instance.CustomType<YearMonthCompositeUserType>(prefix);
				break;
		}
	}
}
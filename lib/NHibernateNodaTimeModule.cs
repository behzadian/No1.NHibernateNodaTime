using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;
using NodaTime;

namespace No1.NHibernateNodaTime;

public static class NHibernateNodaTimeModule
{
	public static AutoPersistenceModel EnableNodaTime(this AutoPersistenceModel convention)
	{
		return convention.Conventions.Add<NodaTimeTypesComponentConvention>();
	}

	public static AutoPersistenceModel EnableNodaTime(this SetupConventionFinder<AutoPersistenceModel> convention)
	{
		return convention.Add<NodaTimeTypesComponentConvention>();
	}

	/*public static void MapNodaProperties<T>(AutoMapping<T> mapping)
	{
		var t = typeof(T);
		foreach (var property in t.GetProperties())
		{
			if(property.PropertyType == typeof(Instant)){
				//mapping.Map()
			}
		}
	}*/

	public static void MapInstantProperty(PropertyPart propertyPart, string propertyName)
	{
		MapColumns<InstantCompositeUserType>(propertyPart, propertyName, InstantCompositeUserType.Columns);
	}

	public static void MapZonedDateTimeProperty(PropertyPart propertyPart, string propertyName)
	{
		MapColumns<ZonedDateTimeCompositeUserType>(propertyPart, propertyName, ZonedDateTimeCompositeUserType.Columns);
	}

	public static void MapDurationProperty(PropertyPart propertyPart, string propertyName)
	{
		MapColumns<DurationCompositeUserType>(propertyPart, propertyName, DurationCompositeUserType.Columns);
	}

	public static void MapAnnualDateProperty(PropertyPart propertyPart, string propertyName)
	{
		MapColumns<AnnualDateCompositeUserType>(propertyPart, propertyName, AnnualDateCompositeUserType.Columns);
	}

	public static void MapLocalDateProperty(PropertyPart propertyPart, string propertyName)
	{
		MapColumns<LocalDateCompositeUserType>(propertyPart, propertyName, LocalDateCompositeUserType.Columns);
	}

	private static void MapColumns<T>(PropertyPart propertyPart, string prefix, string[] columns)
	{
		propertyPart.CustomType<T>();
		propertyPart.Columns.Clear();
		foreach (var property in columns)
		{
			propertyPart.Columns.Add($"{prefix}_{property}");
		}
	}
}
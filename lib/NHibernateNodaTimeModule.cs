using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;

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
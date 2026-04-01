using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;

namespace No1.NHibernateNodaTime;

public static class NHibernateNodaTimeModule
{
	public static AutoPersistenceModel EnableNodaTime(this AutoPersistenceModel convention)
	{
		return convention
			.Conventions.Add<InstantComponentConvention>()
			;
	}

	public static AutoPersistenceModel EnableNodaTime(this SetupConventionFinder<AutoPersistenceModel> convention)
	{
		return convention.Add<InstantComponentConvention>();
	}

	public static void MapInstantProperty(PropertyPart propertyPart, string propertyName)
	{
		propertyPart.CustomType<InstantCompositeUserType>();
		propertyPart.Columns.Clear();
		propertyPart.Columns.Add(propertyName + "_Timestamp");
		propertyPart.Columns.Add(propertyName + "_Nanoseconds");
	}

	public static void MapZonedDateTimeProperty(PropertyPart propertyPart, string propertyName)
	{
		propertyPart.CustomType<ZonedDateTimeCompositeUserType>();
		propertyPart.Columns.Clear();
		propertyPart.Columns.Add(propertyName + "_UTC");
		propertyPart.Columns.Add(propertyName + "_Local");
		propertyPart.Columns.Add(propertyName + "_Nanoseconds");
		propertyPart.Columns.Add(propertyName + "_ZoneID");
	}
}
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;

namespace No1.NHibernateNodaTime;

public static class NHibernateNodaTimeModule
{
	/*public static AutoPersistenceModel EnableNodaTime(this AutoPersistenceModel convention)
    {
        return convention.Conventions.Add<InstantConvention>();
    }
    
    public static AutoPersistenceModel EnableNodaTime(this SetupConventionFinder<AutoPersistenceModel> convention)
    {
        return convention.Add<InstantConvention>();
    }*/
	public static void MapInstantProperty(PropertyPart propertyPart, string propertyName)
	{
		propertyPart.CustomType<InstantCompositeUserType>();
		propertyPart.Columns.Clear();
		propertyPart.Columns.Add(propertyName + "_Timestamp");
		propertyPart.Columns.Add(propertyName + "_Nanoseconds");
	}
}
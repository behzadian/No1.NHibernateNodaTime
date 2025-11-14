using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;

namespace No1.NHibernate.NodaTime;

public static class NHibernateNodaTimeModule
{
    public static AutoPersistenceModel EnableNodaTime(this AutoPersistenceModel convention)
    {
        return convention.Conventions.Add<InstantConvention>();
    }
    
    public static AutoPersistenceModel EnableNodaTime(this SetupConventionFinder<AutoPersistenceModel> convention)
    {
        return convention.Add<InstantConvention>();
    }
}
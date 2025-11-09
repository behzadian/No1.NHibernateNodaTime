using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions;
using NHibernate.Cfg;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;

namespace NHibernate.NodaTime;

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
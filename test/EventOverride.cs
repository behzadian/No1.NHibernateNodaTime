using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests;

public class EventOverride : IAutoMappingOverride<Event>
{
    public void Override(AutoMapping<Event> mapping)
    {
        mapping.Map(x => x.JustInstant)
               .CustomType<InstantCompositeUserType>();
    }
}

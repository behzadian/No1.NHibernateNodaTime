using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests;

public class EventOverride : IAutoMappingOverride<Event>
{
    public void Override(AutoMapping<Event> mapping)
    {
		NHibernateNodaTimeModule.MapInstantProperty(mapping.Map(x => x.JustInstant), nameof(Event.JustInstant));
		NHibernateNodaTimeModule.MapZonedDateTimeProperty(mapping.Map(x => x.AlwaysZdt), nameof(Event.AlwaysZdt));
	}
}

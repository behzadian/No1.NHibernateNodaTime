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
		NHibernateNodaTimeModule.MapInstantProperty(mapping.Map(x => x.InstantValuable), nameof(Event.InstantValuable));
		NHibernateNodaTimeModule.MapZonedDateTimeProperty(mapping.Map(x => x.ZdtValauable), nameof(Event.ZdtValauable));
		NHibernateNodaTimeModule.MapDurationProperty(mapping.Map(x => x.DurationValauable), nameof(Event.DurationValauable));
		NHibernateNodaTimeModule.MapAnnualDateProperty(mapping.Map(x => x.AnnualDateValauable), nameof(Event.AnnualDateValauable));
		NHibernateNodaTimeModule.MapLocalDateProperty(mapping.Map(x => x.LdValauable), nameof(Event.LdValauable));
		NHibernateNodaTimeModule.MapLocalDateTimeProperty(mapping.Map(x => x.LdtValauable), nameof(Event.LdtValauable));
	}
}

using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests.Overrides;

public class ZonedDateTimeOverride : IAutoMappingOverride<ZonedDateTimeEntity>
{
	void IAutoMappingOverride<ZonedDateTimeEntity>.Override(AutoMapping<ZonedDateTimeEntity> mapping)
	{
		NHibernateNodaTimeModule.MapZonedDateTimeProperty(mapping.Map(x => x.Valauable), nameof(ZonedDateTimeEntity.Valauable));
	}
}
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests.Overrides;

public class LocalTimeOverride : IAutoMappingOverride<LocalTimeEntity>
{
    void IAutoMappingOverride<LocalTimeEntity>.Override(AutoMapping<LocalTimeEntity> mapping)
    {
		NHibernateNodaTimeModule.MapLocalDateProperty(mapping.Map(x => x.LtValauable), nameof(LocalTimeEntity.LtValauable));
	}
}

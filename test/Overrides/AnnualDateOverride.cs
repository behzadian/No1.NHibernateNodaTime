using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests.Overrides;

public class AnnualDateOverride : IAutoMappingOverride<AnnualDateEntity>
{
    void IAutoMappingOverride<AnnualDateEntity>.Override(AutoMapping<AnnualDateEntity> mapping)
    {
		NHibernateNodaTimeModule.MapAnnualDateProperty(mapping.Map(x => x.AnnualDateValauable), nameof(AnnualDateEntity.AnnualDateValauable));
	}
}

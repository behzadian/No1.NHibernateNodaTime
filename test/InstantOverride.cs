using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests;

public class InstantOverride : IAutoMappingOverride<InstantEntity>
{
    void IAutoMappingOverride<InstantEntity>.Override(AutoMapping<InstantEntity> mapping)
    {
		NHibernateNodaTimeModule.MapInstantProperty(mapping.Map(x => x.Valuable), nameof(InstantEntity.Valuable));
	}
}

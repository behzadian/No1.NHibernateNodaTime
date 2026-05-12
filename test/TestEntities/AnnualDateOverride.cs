using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;
using static No1.NHibernateNodaTime.NHibernateNodaTimeModule;

namespace No1.NHibernateNodaTimeTests.TestEntities;

public class AnnualDateOverride : IAutoMappingOverride<AnnualDateEntity>
{
	void IAutoMappingOverride<AnnualDateEntity>.Override(AutoMapping<AnnualDateEntity> mapping)
	{
		MapAnnualDateProperty(mapping.Map(x => x.Valauable), nameof(AnnualDateEntity.Valauable));
	}
}
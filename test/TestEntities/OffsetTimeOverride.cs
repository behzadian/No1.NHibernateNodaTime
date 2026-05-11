using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests.Overrides;

public class OffsetTimeOverride : IAutoMappingOverride<OffsetTimeEntity>
{
	void IAutoMappingOverride<OffsetTimeEntity>.Override(AutoMapping<OffsetTimeEntity> mapping)
	{
		NHibernateNodaTimeModule.MapOffsetTimeProperty(mapping.Map(x => x.Valauable), nameof(OffsetTimeEntity.Valauable));
	}
}
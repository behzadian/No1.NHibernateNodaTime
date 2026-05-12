using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests.TestEntities;

public class DurationOverride : IAutoMappingOverride<DurationEntity>
{
	void IAutoMappingOverride<DurationEntity>.Override(AutoMapping<DurationEntity> mapping)
	{
		NHibernateNodaTimeModule.MapDurationProperty(mapping.Map(x => x.Valauable), nameof(DurationEntity.Valauable));
	}
}
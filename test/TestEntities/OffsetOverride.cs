using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests.Overrides;

public class OffsetOverride : IAutoMappingOverride<OffsetEntity>
{
	void IAutoMappingOverride<OffsetEntity>.Override(AutoMapping<OffsetEntity> mapping)
	{
		NHibernateNodaTimeModule.MapOffsetProperty(mapping.Map(x => x.Valauable), nameof(OffsetEntity.Valauable));
	}
}
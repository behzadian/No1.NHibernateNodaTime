using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests.TestEntities;

public class OffsetDateTimeOverride : IAutoMappingOverride<OffsetDateTimeEntity>
{
	void IAutoMappingOverride<OffsetDateTimeEntity>.Override(AutoMapping<OffsetDateTimeEntity> mapping)
	{
		NHibernateNodaTimeModule.MapOffsetDateTimeProperty(mapping.Map(x => x.Valauable), nameof(OffsetDateTimeEntity.Valauable));
	}
}
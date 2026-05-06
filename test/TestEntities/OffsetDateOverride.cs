using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests.Overrides;

public class OffsetDateOverride : IAutoMappingOverride<OffsetDateEntity>
{
    void IAutoMappingOverride<OffsetDateEntity>.Override(AutoMapping<OffsetDateEntity> mapping)
    {
		NHibernateNodaTimeModule.MapOffsetDateProperty(mapping.Map(x => x.Valauable), nameof(OffsetDateEntity.Valauable));
	}
}

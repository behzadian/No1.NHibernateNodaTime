using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests.TestEntities;

public class LocalDateOverride : IAutoMappingOverride<LocalDateEntity>
{
	void IAutoMappingOverride<LocalDateEntity>.Override(AutoMapping<LocalDateEntity> mapping)
	{
		NHibernateNodaTimeModule.MapLocalDateProperty(mapping.Map(x => x.Valauable), nameof(LocalDateEntity.Valauable));
	}
}
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests.TestEntities;

public class LocalDateTimeOverride : IAutoMappingOverride<LocalDateTimeEntity>
{
	void IAutoMappingOverride<LocalDateTimeEntity>.Override(AutoMapping<LocalDateTimeEntity> mapping)
	{
		NHibernateNodaTimeModule.MapLocalDateTimeProperty(mapping.Map(x => x.Valauable), nameof(LocalDateTimeEntity.Valauable));
	}
}
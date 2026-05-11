using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace No1.NHibernateNodaTimeTests.Overrides;

public class YearMonthOverride : IAutoMappingOverride<YearMonthEntity>
{
	void IAutoMappingOverride<YearMonthEntity>.Override(AutoMapping<YearMonthEntity> mapping)
	{
		NHibernateNodaTimeModule.MapYearMonthProperty(mapping.Map(x => x.Valauable), nameof(YearMonthEntity.Valauable));
	}
}
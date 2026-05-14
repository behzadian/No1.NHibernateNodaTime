using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace No1.NHibernateNodaTimeTests.Conventions;

public class GuidTypeConvention : IUserTypeConvention
{
	public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
	{
		criteria.Expect(x => x.Property.PropertyType == typeof(Guid) || x.Property.PropertyType == typeof(Guid?));
	}

	public void Apply(IPropertyInstance instance)
	{
		instance.CustomSqlType("uuid");
	}
}
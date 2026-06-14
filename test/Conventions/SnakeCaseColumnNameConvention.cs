using JetBrains.Annotations;
using No1.FaraBank.Api.Repos.Conventions;

namespace No1.NHibernateNodaTimeTests.Conventions;

using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

[UsedImplicitly]
public class SnakeCaseColumnNameConvention : IPropertyConvention
{
	public void Apply(IPropertyInstance instance) {
		instance.Column(instance.Property.Name.SnakeCase());
	}
}
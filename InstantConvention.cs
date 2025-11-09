using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using JetBrains.Annotations;
using NodaTime;

namespace NHibernate.NodaTime;

[UsedImplicitly]
public class InstantConvention : UserTypeConvention<InstantUserType>
{
}

using FluentNHibernate.Conventions;
using JetBrains.Annotations;

namespace No1.NHibernateNodaTime;

[UsedImplicitly]
public class InstantConvention : UserTypeConvention<InstantUserType>
{
}

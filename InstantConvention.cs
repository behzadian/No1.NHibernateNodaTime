using FluentNHibernate.Conventions;
using JetBrains.Annotations;

namespace No1.NHibernate.NodaTime;

[UsedImplicitly]
public class InstantConvention : UserTypeConvention<InstantUserType>
{
}

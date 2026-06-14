using System.Runtime.CompilerServices;

namespace No1.NHibernateNodaTime;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Other contructors are not needed")]
public class UnsupportedValueException(
	object? value,
	[CallerArgumentExpression(nameof(value))] string label = ""
) : Exception($"value `{value}` is not accepted value for `{label}`.")
{
}
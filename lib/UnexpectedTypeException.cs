using System.Runtime.CompilerServices;

namespace No1.NHibernateNodaTime;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Other contructors are not needed")]
public class UnexpectedTypeException<T>(
	object? value,
	[CallerArgumentExpression(nameof(value))] string label = ""
) : Exception($"{label} expected to be {typeof(T).Name}, but is `{value?.GetType()?.Name ?? "<NULL>"}`. Value is `{value}`")
{
}
namespace No1.NHibernateNodaTime;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
public class UnexpectedTypeException(string label, object value, Type expectedType) : Exception($"{label} expected to be {expectedType.Name}, but is {value.GetType().Name}. Value is `{value}`")
{

}
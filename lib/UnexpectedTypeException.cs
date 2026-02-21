namespace No1.NHibernateNodaTime;

public class UnexpectedTypeException(string label, object value, Type expectedType) : Exception($"{label} expected to be {expectedType.Name}, but is {value.GetType().Name}. Value is `{value}`")
{
    
}
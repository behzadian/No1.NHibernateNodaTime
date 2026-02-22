using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

/// <summary>
/// Composite UserType that stores NodaTime Instant as two separate columns:
/// - Seconds since Unix epoch (long)
/// - Nanoseconds component (int)
/// </summary>
public class InstantCompositeUserType : ICompositeUserType
{
    public Type ReturnedClass => typeof(Instant?);

    public bool IsMutable => false;

    public string[] PropertyNames => ["Seconds", "Nanoseconds"];

    public IType[] PropertyTypes =>
    [
        NHibernateUtil.Int64,  // Seconds
        NHibernateUtil.Int32   // Nanoseconds
    ];

    public object? NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
    {
        var seconds = dr[names[0]] as long?;
        var nanoseconds = dr[names[1]] as int?;

        if (seconds == null || nanoseconds == null)
            return null;

        return Instant.FromUnixTimeSeconds(seconds.Value).PlusNanoseconds(nanoseconds.Value);
    }

    public void NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session)
    {
        if (value == null)
        {
            NHibernateUtil.Int64.NullSafeSet(cmd, null, index, session);
            NHibernateUtil.Int32.NullSafeSet(cmd, null, index + 1, session);
        }
        else
        {
            var instant = (Instant)value;
            (long seconds, int nanoseconds) = instant.ToUnixTimeSecondsAndNanoseconds();

            NHibernateUtil.Int64.NullSafeSet(cmd, seconds, index, session);
            NHibernateUtil.Int32.NullSafeSet(cmd, nanoseconds, index + 1, session);
        }
    }

    public object GetPropertyValue(object component, int property)
    {
        var instant = (Instant)component;
        (long seconds, int nanoseconds) = instant.ToUnixTimeSecondsAndNanoseconds();
        return property switch
        {
            0 => seconds,
            1 => nanoseconds,
            _ => throw new ArgumentOutOfRangeException(nameof(property))
        };
    }

    public void SetPropertyValue(object component, int property, object value)
    {
        throw new InvalidOperationException("Instant is immutable");
    }

    public object DeepCopy(object value)
    {
        // Instant is immutable
        return value;
    }

    public object Disassemble(object value, ISessionImplementor session)
    {
        return value;
    }

    public object Assemble(object cached, ISessionImplementor session, object owner)
    {
        return cached;
    }

    public object Replace(object original, object target, ISessionImplementor session, object owner)
    {
        return original;
    }

    public new bool Equals(object? x, object? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;
        return ((Instant)x).Equals((Instant)y);
    }

    public int GetHashCode(object? x)
    {
        return x?.GetHashCode() ?? 0;
    }
}
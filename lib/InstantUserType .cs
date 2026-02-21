using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

public class InstantUserType : IUserType
{
    public SqlType[] SqlTypes => [NHibernateUtil.Int64.SqlType,NHibernateUtil.Int64.SqlType];

    public System.Type ReturnedType => typeof(Instant);

    public bool IsMutable => false;

    public object? NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
    {
        var secVal = NHibernateUtil.Int64.NullSafeGet(rs, names[0], session);
        var nanVal = NHibernateUtil.Int64.NullSafeGet(rs, names[1], session);
        return (secVal,nanVal) switch
        {
            (null,null) => null,
            (null,_) => null,
            (_,null) => null,
            (long seconds,long nanoseconds) => Instant.FromUnixTimeTicks(seconds).PlusNanoseconds(nanoseconds),
            _ => throw new UnexpectedTypeException($"Stored value for Instant's second",secVal, typeof(long))
        };
    }

    public void NullSafeSet(DbCommand cmd, object? value, int index, ISessionImplementor session)
    {
        if (value == null)
        {
            NHibernateUtil.Int64.NullSafeSet(cmd, null, index, session);
            NHibernateUtil.Int64.NullSafeSet(cmd, null, index+1, session);
        }
        else
        {
            (long seconds, int nanoseconds) = ((Instant)value).ToUnixTimeSecondsAndNanoseconds();
            NHibernateUtil.Int64.NullSafeSet(cmd, seconds, index, session);
            NHibernateUtil.Int64.NullSafeSet(cmd, nanoseconds, index, session);
        }
    }

    public object DeepCopy(object value)
    {
        return value;
    }

    public object Replace(object original, object target, object owner)
    {
        return original;
    }

    public object Assemble(object cached, object owner)
    {
        return cached;
    }

    public object Disassemble(object value)
    {
        return value;
    }

    public new bool Equals(object x, object y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;
        return x.Equals(y);
    }

    public int GetHashCode(object x)
    {
        return x?.GetHashCode() ?? 0;
    }
}

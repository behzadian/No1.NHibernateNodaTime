using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace NHibernate.NodaTime;

public class LocalDateUserType : IUserType
{
    public SqlType[] SqlTypes => [NHibernateUtil.DateTime.SqlType];

    public System.Type ReturnedType => typeof(Instant);

    public bool IsMutable => false;

    public object? NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
    {
        var value = NHibernateUtil.Int64.NullSafeGet(rs, names[0], session);
        if (value == null) return null;

        var epoch = (long)value;
        return Instant.FromUnixTimeTicks(epoch);
    }

    public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
    {
        if (value == null)
        {
            NHibernateUtil.DateTime.NullSafeSet(cmd, null, index, session);
        }
        else
        {
            var instant = (Instant)value;
            NHibernateUtil.DateTime.NullSafeSet(cmd, instant.ToUnixTimeTicks(), index, session);
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

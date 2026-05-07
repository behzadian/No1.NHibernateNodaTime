using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;
using NodaTime;
using System.Data.Common;
using static No1.NHibernateNodaTime.NodaTimeUtility;

namespace No1.NHibernateNodaTime;

/// <summary>
/// </summary>
public sealed class LocalTimeUserType : IUserType
{
	public static readonly IUserType Instance = new LocalTimeUserType();

	public static readonly IType NHType = NHibernateUtil.Custom(typeof(LocalTimeUserType));

	internal static string[] Columns => ["TimeNanos",];

	SqlType[] IUserType.SqlTypes => [NHibernateUtil.Int64.SqlType];

	Type IUserType.ReturnedType => typeof(LocalTime);

	bool IUserType.IsMutable => false;

	bool IUserType.Equals(object x, object y)
	{
		if (ReferenceEquals(x, y)) return true;
		if (x == null || y == null) return false;
		return ((LocalTime)x).Equals((LocalTime)y);
	}

	int IUserType.GetHashCode(object x)
	{
		return x?.GetHashCode() ?? 0;
	}

	object? IUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
	{
		var counter = 0;

		if (dr[names[counter++]] is not long nanos)
			return null;

		return LocalTime.FromNanosecondsSinceMidnight(nanos);
	}

	void IUserType.NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
	{
		if (value is LocalTime lt)
		{
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, lt.NanosecondOfDay, counter++, session);
		}
		else
		{
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object IUserType.DeepCopy(object value)
	{
		return value;
	}

	object IUserType.Replace(object original, object target, object owner)
	{
		throw new NotImplementedException();
	}

	object IUserType.Assemble(object cached, object owner)
	{
		throw new NotImplementedException();
	}

	object IUserType.Disassemble(object value)
	{
		throw new NotImplementedException();
	}
}
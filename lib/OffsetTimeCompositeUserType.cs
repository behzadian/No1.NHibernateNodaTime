using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;
using NodaTime;
using System.Data.Common;
using static No1.NHibernateNodaTime.NodaTimeUtility;

namespace No1.NHibernateNodaTime;

/// <summary>
/// </summary>
public sealed class OffsetTimeCompositeUserType : ICompositeUserType
{
	Type ICompositeUserType.ReturnedClass => typeof(OffsetTime?);

	bool ICompositeUserType.IsMutable => false;

	internal static readonly string[] Columns = [.. LocalTimeUserType.Columns, .. OffsetUserType.Columns];

	internal static readonly int TimeColumnsCount = LocalTimeUserType.Columns.Length;

	string[] ICompositeUserType.PropertyNames => Columns;


	IType[] ICompositeUserType.PropertyTypes =>
	[
		LocalTimeUserType.NHType,
		OffsetUserType.NHType
	];

	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner) {
		var timeNames = names[..1];
		var offsetNames = names[1..];

		var time = (LocalTime?)LocalTimeUserType.Instance.NullSafeGet(dr, timeNames, session, owner);

		var offset = (Offset?)OffsetUserType.Instance.NullSafeGet(dr, offsetNames, session, owner);

		if (time is null || offset is null) return null;

		return new OffsetTime(time.Value, offset.Value);
	}

	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session) {
		if (value is OffsetTime val) {
			LocalTimeUserType.Instance.NullSafeSet(cmd, val.TimeOfDay, index, session);
			OffsetUserType.Instance.NullSafeSet(cmd, val.Offset, index + TimeColumnsCount, session);
		} else {
			LocalTimeUserType.Instance.NullSafeSet(cmd, null, index, session);
			OffsetUserType.Instance.NullSafeSet(cmd, null, index + TimeColumnsCount, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property) {
		if (component is OffsetTime val) {
			return property < TimeColumnsCount ? val.TimeOfDay : val.Offset;
		} else {
			throw new UnexpectedTypeException<OffsetTime>(component);
		}
	}

	void ICompositeUserType.SetPropertyValue(object component, int property, object value) {
		throw new InvalidOperationException("immutable");
	}

	object ICompositeUserType.DeepCopy(object value) {
		return value;
	}

	object ICompositeUserType.Disassemble(object value, ISessionImplementor session) {
		return value;
	}

	object ICompositeUserType.Assemble(object cached, ISessionImplementor session, object owner) {
		return cached;
	}

	object ICompositeUserType.Replace(object original, object target, ISessionImplementor session, object owner) {
		return original;
	}

	bool ICompositeUserType.Equals(object? x, object? y) {
		if (ReferenceEquals(x, y)) return true;
		if (x == null || y == null) return false;
		return ((OffsetTime)x).Equals((OffsetTime)y);
	}

	int ICompositeUserType.GetHashCode(object? x) {
		return x?.GetHashCode() ?? 0;
	}
}
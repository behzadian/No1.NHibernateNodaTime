using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

public sealed class OffsetDateTimeCompactUserType : ICompositeUserType
{
	internal static readonly string[] Columns = [.. LocalDateTimeCompactUserType.Columns, .. OffsetUserType.Columns];

	private static readonly int DateTimeColumnsCount = LocalDateTimeCompactUserType.Columns.Length;

	Type ICompositeUserType.ReturnedClass => typeof(OffsetDateTime?);

	bool ICompositeUserType.IsMutable => false;

	string[] ICompositeUserType.PropertyNames => Columns;

	IType[] ICompositeUserType.PropertyTypes =>
	[
		..LocalDateTimeCompactUserType.Instance.PropertyTypes,
		OffsetUserType.NHType
	];

	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner) {
		// Split names between date and time parts
		var dateNames = names[..DateTimeColumnsCount];
		var timeName = names[DateTimeColumnsCount..];

		var date = (LocalDateTime?)LocalDateTimeCompactUserType.Instance.NullSafeGet(dr, dateNames, session, owner);

		var offset = (Offset?)OffsetUserType.Instance.NullSafeGet(dr, timeName, session, owner);

		if (date is null || offset is null) {
			return null;
		}

		return new OffsetDateTime(date.Value, offset.Value);
	}

	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session) {
		if (value is OffsetDateTime val) {
			LocalDateTimeCompactUserType.Instance.NullSafeSet(cmd, val.LocalDateTime, index, settable, session);
			OffsetUserType.Instance.NullSafeSet(cmd, val.Offset, index + DateTimeColumnsCount, session);
		} else {
			LocalDateTimeCompactUserType.Instance.NullSafeSet(cmd, null, index, settable, session);
			OffsetUserType.Instance.NullSafeSet(cmd, null, index + DateTimeColumnsCount, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property) {
		if (component is OffsetDateTime val) {
			return property < DateTimeColumnsCount ? LocalDateTimeCompactUserType.Instance.GetPropertyValue(val.LocalDateTime, property) : val.Offset;
		} else {
			throw new UnexpectedTypeException<OffsetDateTime>(component);
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
		if (ReferenceEquals(x, y)) {
			return true;
		}

		if (x == null || y == null) {
			return false;
		}

		return ((OffsetDateTime)x).Equals((OffsetDateTime)y);
	}

	int ICompositeUserType.GetHashCode(object? x) {
		return x?.GetHashCode() ?? 0;
	}
}
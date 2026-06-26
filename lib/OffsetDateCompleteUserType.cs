using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

public sealed class OffsetDateCompleteUserType : ICompositeUserType
{
	internal static readonly string[] Columns = [.. LocalDateCompleteUserType.Columns, .. OffsetUserType.Columns];

	private static readonly int DateColumnsCount = LocalDateCompleteUserType.Columns.Length;

	Type ICompositeUserType.ReturnedClass => typeof(OffsetDate?);

	bool ICompositeUserType.IsMutable => false;

	string[] ICompositeUserType.PropertyNames => Columns;

	IType[] ICompositeUserType.PropertyTypes =>
	[
		..LocalDateCompleteUserType.Instance.PropertyTypes,
		OffsetUserType.NHType
	];

	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner) {
		// Split names between date and time parts
		var dateNames = names[..DateColumnsCount];
		var timeName = names[DateColumnsCount..];

		var date = (LocalDate?)LocalDateCompleteUserType.Instance.NullSafeGet(dr, dateNames, session, owner);

		var time = (Offset?)OffsetUserType.Instance.NullSafeGet(dr, timeName, session, owner);

		if (date is null || time is null) {
			return null;
		}

		OffsetDate value = new(date.Value, time.Value);
		return value;
	}

	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session) {
		if (value is OffsetDate ldt) {
			LocalDateCompleteUserType.Instance.NullSafeSet(cmd, ldt.Date, index, settable, session);
			OffsetUserType.Instance.NullSafeSet(cmd, ldt.Offset, index + DateColumnsCount, session);
		} else {
			LocalDateCompleteUserType.Instance.NullSafeSet(cmd, null, index, settable, session);
			OffsetUserType.Instance.NullSafeSet(cmd, null, index + DateColumnsCount, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property) {
		if (component is OffsetDate val) {
			return property < DateColumnsCount ? LocalDateCompleteUserType.Instance.GetPropertyValue(val.Date, property) : val.Offset;
		} else {
			throw new UnexpectedTypeException<OffsetDate>(component);
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

		return ((OffsetDate)x).Equals((OffsetDate)y);
	}

	int ICompositeUserType.GetHashCode(object? x) {
		return x?.GetHashCode() ?? 0;
	}
}
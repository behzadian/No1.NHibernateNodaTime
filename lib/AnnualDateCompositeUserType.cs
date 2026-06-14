using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

public sealed class AnnualDateCompositeUserType : ICompositeUserType
{
	internal static readonly string[] Columns = ["Month", "Day",];

	Type ICompositeUserType.ReturnedClass => typeof(AnnualDate?);

	bool ICompositeUserType.IsMutable => false;

	string[] ICompositeUserType.PropertyNames => Columns;

	IType[] ICompositeUserType.PropertyTypes =>
	[
		NHibernateUtil.Int16,
		NHibernateUtil.Int16,
	];

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulness")]
	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner) {
		var counter = 0;

		if (dr[names[counter++]] is not short month) {
			return null;
		}

		if (dr[names[counter++]] is not short day) {
			return null;
		}

		return new AnnualDate(month, day);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulness")]
	void ICompositeUserType.NullSafeSet(DbCommand cmd, object? value, int index, bool[] settable, ISessionImplementor session) {
		if (value is AnnualDate ad) {
			var counter = index;
			NHibernateUtil.Int16.NullSafeSet(cmd, ad.Month, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, ad.Day, counter++, session);
		} else {
			var counter = index;
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int16.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property) {
		if (component is AnnualDate val) {
			return property switch {
				0 => val.Month,
				1 => val.Day,
				_ => throw new ArgumentOutOfRangeException(nameof(property)),
			};
		} else {
			throw new UnexpectedTypeException<AnnualDate>(component);
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

		return ((AnnualDate)x).Equals((AnnualDate)y);
	}

	int ICompositeUserType.GetHashCode(object? x) {
		return x?.GetHashCode() ?? 0;
	}
}
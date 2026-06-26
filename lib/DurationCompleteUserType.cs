using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;
using System.Data.Common;

namespace No1.NHibernateNodaTime;

public sealed class DurationCompleteUserType : ICompositeUserType
{
	internal static readonly string[] Columns = ["Seconds", "Nanos"];

	bool ICompositeUserType.IsMutable => false;

	string[] ICompositeUserType.PropertyNames => Columns;

	IType[] ICompositeUserType.PropertyTypes => [NHibernateUtil.Int64, NHibernateUtil.Int32];

	Type ICompositeUserType.ReturnedClass => typeof(Duration?);

	object ICompositeUserType.Assemble(object cached, ISessionImplementor session, object owner) {
		return cached;
	}

	object ICompositeUserType.DeepCopy(object value) {
		return value;
	}

	object ICompositeUserType.Disassemble(object value, ISessionImplementor session) {
		return value;
	}

	bool ICompositeUserType.Equals(object x, object y) {
		return x?.Equals(y) ?? false;
	}

	int ICompositeUserType.GetHashCode(object x) {
		return x?.GetHashCode() ?? 0;
	}

	object? ICompositeUserType.GetPropertyValue(object component, int property) {
		if (component is Duration duration) {
			return property switch {
				0 => duration.TotalSeconds,
				1 => duration.SubsecondNanoseconds,
				_ => throw new NotImplementedException(),
			};
		} else {
			throw new UnexpectedTypeException<Duration>(component);
		}
	}

	object? ICompositeUserType.NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner) {
		if (dr[names[0]] is not long secs) {
			return null;
		}

		if (dr[names[1]] is not int nanos) {
			return null;
		}

		return Duration.FromSeconds(secs).Plus(Duration.FromNanoseconds(nanos));
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Beautifulness")]
	void ICompositeUserType.NullSafeSet(DbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session) {
		if (value is Duration duration) {
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, (long)(duration.ToInt128Nanoseconds() / (long)Math.Pow(10, 9)), counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, duration.SubsecondNanoseconds, counter++, session);
		} else {
			var counter = index;
			NHibernateUtil.Int64.NullSafeSet(cmd, null, counter++, session);
			NHibernateUtil.Int32.NullSafeSet(cmd, null, counter++, session);
		}
	}

	object ICompositeUserType.Replace(object original, object target, ISessionImplementor session, object owner) {
		throw new NotImplementedException();
	}

	void ICompositeUserType.SetPropertyValue(object component, int property, object value) {
		throw new NotImplementedException();
	}
}
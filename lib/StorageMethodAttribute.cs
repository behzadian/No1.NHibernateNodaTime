using System.Reflection;

namespace No1.NHibernateNodaTime;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class StorageMethodAttribute(StorageMethods method = StorageMethods.Compact) : Attribute
{
	public StorageMethods Method { get; } = method;

	internal static bool CompactStorageEnabled(PropertyInfo property) {
		return GetAttributeHirarachy(property) == StorageMethods.Compact;
	}

	internal static bool CompleteStorageEnabled(PropertyInfo property) {
		return GetAttributeHirarachy(property) == StorageMethods.Complete;
	}

	private static StorageMethods GetAttributeHirarachy(PropertyInfo property) {
		return property.GetCustomAttribute<StorageMethodAttribute>()?.Method ?? ClassAttribute(property);
	}

	private static StorageMethods ClassAttribute(PropertyInfo property) {
		return property.DeclaringType?.GetCustomAttribute<StorageMethodAttribute>()?.Method ?? AssemblyAttribute(property);
	}

	private static StorageMethods AssemblyAttribute(PropertyInfo property) {
		return property.DeclaringType?.Assembly?.GetCustomAttribute<StorageMethodAttribute>()?.Method ?? StorageMethods.Compact;
	}
}
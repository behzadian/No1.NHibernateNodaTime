using System.Reflection;

namespace No1.NHibernateNodaTime;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed partial class StorageMethodAttribute(StorageMethods method = StorageMethods.Simple) : Attribute
{
	public StorageMethods Method { get; } = method;

	internal static bool SimpleStorageEnabled(PropertyInfo property) {
		var attribute = property.GetCustomAttribute<StorageMethodAttribute>();
		return attribute?.Method != StorageMethods.Precise;
	}

	internal static bool PreciseStorageEnabled(PropertyInfo property) {
		var attribute = property.GetCustomAttribute<StorageMethodAttribute>();
		return attribute?.Method == StorageMethods.Precise;
	}
}
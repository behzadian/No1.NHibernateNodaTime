# No1.NHibernateNodaTime
This library persists all NodaTime types into database using NHibernate.

Types that can be stored in database are:
- AnnualDateTimeCompositeUserType
- Duration
- Instant
- LocalDate
- LocalDateTime
- LocalTime
- OffsetDateTime
- OffsetDate
- OffsetTime
- Offset
- Period
- YearMonth

## Usage
First run below command to add `No1.NHibernateNodaTime` dependency to your module:
```shell
dotnet add package No1.NHibernateNodaTime --version # find latest version at https://www.nuget.org/packages/No1.NHibernateNodaTime/
```

If you use FluentAutoMapping, almost anything will be done automatically. Use below code to setup auto mapping:
```
using No1.NHibernateNodaTime;
....
var nhibernateConfig = Fluently.Configure()
	.Database(PostgreSQLConfiguration.Standard
		.ConnectionString(_container.GetConnectionString())
		.ShowSql()
		.FormatSql())
	.Mappings(m => m
		.AutoMappings
		.Add(AutoMap
			.EnableNodaTime() // <--- here
			.UseOverridesFromAssemblyOf<YourEntitiesAssembly>()
		 )
	)
	.BuildConfiguration();
```
This code add NodeTime type convertions to NHibernate config. 

### Override
There is a problem with Automap. Automap, maps unnullable property types like `Instant` (In compare to `Instant?`) as an entity.


Imagine this is your entity:

```
public class [Your Entity]
{
	public virtual int Id { get; set; }
	public virtual Instant Valauable { get; set; } // You only need override for this property
	public virtual Instant? Nullable { get; set; } // You do not need override for this property
}
```

We need to override this behaviour, so create an override class for your entities (like below):

```csharp
public class Overrides :
	IAutoMappingOverride<Entity1>,
	IAutoMappingOverride<Entity2>
{
	void IAutoMappingOverride<Entity1>.Override(AutoMapping<Entity1> mapping) {
		NodaTimeUtility.OverrideEntity(mapping);
	}
	
	void IAutoMappingOverride<Entity2>.Override(AutoMapping<Entity2> mapping) {
		NodaTimeUtility.OverrideEntity(mapping);
	}
}
```

`NodaTimeUtility.OverrideEntity(mapping);` scans your entity and find all properties with supported NodaTime types, then overrides their configuration.

Please remember to add your override to NHibernate AutoMap configuration:

```
using No1.NHibernateNodaTime;
....
var nhibernateConfig = Fluently.Configure()
	.Database(PostgreSQLConfiguration.Standard
		.ConnectionString(_container.GetConnectionString())
		.ShowSql()
		.FormatSql())
	.Mappings(m => m
		.AutoMappings
		.Add(AutoMap
			.EnableNodaTime()
			.UseOverridesFromAssemblyOf<YourEntitiesAssembly>() // <--- here
		 )
	)
	.BuildConfiguration();
```

## Storage options

Because NodaTime's types ranges are bigger than .NET and sql types, they can be stored in one column. 
In addition to range, sometime you want to pay more for storage, but store all queryable information in database, so they can
be involved in queries. For example, Instant's Nanoseconds need extra column, and also I prefer to store its timestamp in addition to Seconds and Nanoseconds.
But in many cases, you may need to store in minimum columns.

So I developed almost 2 user types for any NodaTime types, one as Compact (with minimal columns) and one as Complete (with many columns).

with below attribute, you can decide which usertype to be applied on your property.

```csharp
[StorageMethod(StorageMethods.Compact | StorageMethods.Complete)]
```

This attribute can be applied on Properties, classes, and even assemblies.

If you use `NodaTimeUtility.OverrideEntity(mapping);`, method reads this attribute that is applied on property.
If there was no `StorageMethod` applied on property, then reads the class attribute and then the entities assembly attributes.
As soon as `StorageMethod` found, uses the specified method to detect which user type to use for that specific property.

If no StorageMethod found, the default value is `StorageMethods.Compact`. 
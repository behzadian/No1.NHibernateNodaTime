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
If you use FluentAutoMapping, almost anything will be done automatically. Use below code to setup auto mapping:
```
.Add(AutoMap
	.AssemblyOf<YOUR_ASSEMBLY>(new AutoMappingConfiguration()/*this is default implementation*/)
	.EnableNodaTime() // <- this is required
	.UseOverridesFromAssemblyOf<YOUR_ASSEMBLY>() // <- You must override configuration for every entity that has unnullable property type
)
```

## Override
Automap, maps unnullable property types like `Instant` (In compare to `Instant?`) as an entity. For overriding this behaviour, you must override as below:

Imagine this is your entity:

```
public class [Your Entity]
{
	public virtual int Id { get; set; }
	public virtual Instant Valauable { get; set; } // You only need override for this property
	public virtual Instant? Nullable { get; set; } // You do not need override for this property
}
```

This would be your override class:

```
using static No1.NHibernateNodaTime.NHibernateNodaTimeModule;
public class [Your Entity]Override : IAutoMappingOverride<[Your Entity]>
{
	void IAutoMappingOverride<[Your Entity]>.Override(AutoMapping<[Your Entity]> mapping)
	{
		Map[Unnullable Type]Property(mapping.Map(x => x.Valauable), nameof([Your Entity].Valauable));
	}
}
```
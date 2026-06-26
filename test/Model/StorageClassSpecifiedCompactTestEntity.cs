using No1.NHibernateNodaTime;
using NodaTime;

namespace No1.NHibernateNodaTimeTests.Model;

[StorageMethod(StorageMethods.Compact)]
public class StorageClassSpecifiedCompactTestEntity
{
	public virtual int Id { get; set; }

	[StorageMethod(StorageMethods.Complete)]
	public virtual Duration CompleteDuration { get; set; }

	[StorageMethod(StorageMethods.Compact)]
	public virtual Duration CompactDuration { get; set; }
	public virtual Duration UnspecifiedDuration { get; set; }

	//================

	[StorageMethod(StorageMethods.Complete)]
	public virtual Instant CompleteInstant { get; set; }

	[StorageMethod(StorageMethods.Compact)]
	public virtual Instant CompactInstant { get; set; }
	public virtual Instant UnspecifiedInstant { get; set; }

	//================

	[StorageMethod(StorageMethods.Complete)]
	public virtual LocalDate CompleteLocalDate { get; set; }

	[StorageMethod(StorageMethods.Compact)]
	public virtual LocalDate CompactLocalDate { get; set; }
	public virtual LocalDate UnspecifiedLocalDate { get; set; }

	//================

	[StorageMethod(StorageMethods.Complete)]
	public virtual LocalDateTime CompleteLocalDateTime { get; set; }

	[StorageMethod(StorageMethods.Compact)]
	public virtual LocalDateTime CompactLocalDateTime { get; set; }
	public virtual LocalDateTime UnspecifiedLocalDateTime { get; set; }

	//================

	[StorageMethod(StorageMethods.Complete)]
	public virtual OffsetDate CompleteOffsetDate { get; set; }

	[StorageMethod(StorageMethods.Compact)]
	public virtual OffsetDate CompactOffsetDate { get; set; }
	public virtual OffsetDate UnspecifiedOffsetDate { get; set; }

	//================

	[StorageMethod(StorageMethods.Complete)]
	public virtual OffsetDateTime CompleteOffsetDateTime { get; set; }

	[StorageMethod(StorageMethods.Compact)]
	public virtual OffsetDateTime CompactOffsetDateTime { get; set; }
	public virtual OffsetDateTime UnspecifiedOffsetDateTime { get; set; }

	//================

	[StorageMethod(StorageMethods.Complete)]
	public virtual ZonedDateTime CompleteZonedDateTime { get; set; }

	[StorageMethod(StorageMethods.Compact)]
	public virtual ZonedDateTime CompactZonedDateTime { get; set; }
	public virtual ZonedDateTime UnspecifiedZonedDateTime { get; set; }
}
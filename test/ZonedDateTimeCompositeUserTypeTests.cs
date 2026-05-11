using FluentAssertions;
using NHibernate;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Tests for InstantCompositeUserType that stores Instant in two columns
/// </summary>
public class ZonedDateTimeCompositeUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistZonedDateTimeIn5Columns()
	{
		// Arrange
		var zdt = Instant.FromUtc(2024, 12, 25, 10, 30, 45).InZone(DateTimeZoneProviders.Tzdb["Asia/Tehran"]);
		var entity = new ZonedDateTimeEntity() { Name = "Test Event", Valauable = zdt };

		// Act - Save
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		// Act - Verify in database (check columns were created)
		using (var session = _sessionFactory.OpenSession())
		{
			var sql = @"
				SELECT Valauable_Seconds, Valauable_Nanoseconds, Valauable_ZoneID, Valauable_UTC, Valauable_Local
				FROM ""ZonedDateTimeEntity""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();


			var counter = 0;
			var seconds = Convert.ToInt64(result[counter++]);
			var nanos = Convert.ToInt32(result[counter++]);
			var zone = Convert.ToString(result[counter++]);
			var utc = Convert.ToDateTime(result[counter++]);
			var local = Convert.ToDateTime(result[counter++]);

			// Assert - Verify raw column values
			seconds.Should().Be(zdt.ToInstant().ToUnixTimeSecondsAndNanoseconds().seconds);
			nanos.Should().Be(zdt.ToInstant().ToUnixTimeSecondsAndNanoseconds().nanoseconds);
			zone.Should().Be("Asia/Tehran");
			utc.Should().Be(new DateTime(new DateOnly(2024, 12, 25), new TimeOnly(10, 30, 45), DateTimeKind.Local));
			local.Should().Be(new DateTime(new DateOnly(2024, 12, 25), new TimeOnly(14, 0, 45), DateTimeKind.Utc));
		}

		// Act - Retrieve via NHibernate
		ZonedDateTimeEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<ZonedDateTimeEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.Valauable.Should().Be(zdt);
	}

	[Fact]
	public async Task ShouldPreserveNanosecondPrecision()
	{
		var zdt = Instant
			.FromUnixTimeSeconds(1609459200) // 2021-01-01 00:00:00
			.PlusNanoseconds(123456789)
			.InUtc();

		var entity = new ZonedDateTimeEntity() { Name = "Precision Test", Valauable = zdt };

		// Act
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		ZonedDateTimeEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<ZonedDateTimeEntity>(savedId);
		}

		// Assert
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.Valauable.Should().Be(zdt);
		retrievedEvent.Valauable.ToInstant().OnlyNanoseconds().Should().Be(123456789);
	}

	[Fact]
	public async Task ShouldHandleNullable()
	{
		// Arrange
		var now = SystemClock.Instance.GetCurrentInstant().InUtc();
		var entity = new ZonedDateTimeEntity() { Name = "Test", Valauable = now, Nullable = null };

		// Act - Save without ModifiedAt
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		// Assert - Both columns should be NULL
		using (var session = _sessionFactory.OpenSession())
		{
			var sql = @"
				SELECT Nullable_Seconds, Nullable_Nanoseconds, Nullable_ZoneID, Nullable_UTC, Nullable_Local
				FROM ""ZonedDateTimeEntity""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			for (int i = 0; i < result.Length; i++)
			{
				result[i].Should().BeNull();
			}
		}
	}

	[Fact]
	public async Task ShouldHandleMin()
	{
		// Arrange
		var min = Instant.MinValue.InUtc();

		var minEntity = new ZonedDateTimeEntity() { Name = "Min", Nullable = min };

		// Act
		int minId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			minId = (int)await session.SaveAsync(minEntity);
			await transaction.CommitAsync();
		}

		// Assert
		ZonedDateTimeEntity? retrievedMin;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMin = await session.GetAsync<ZonedDateTimeEntity>(minId);
		}

		retrievedMin!.Nullable.Should().Be(min);
	}

	[Fact]
	public async Task ShouldHandleMax()
	{
		// Arrange
		var max = Instant.MaxValue.InUtc();

		var maxEntity = new ZonedDateTimeEntity() { Name = "Max", Nullable = max };

		// Act
		int maxId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			maxId = (int)await session.SaveAsync(maxEntity);
			await transaction.CommitAsync();
		}

		// Assert
		ZonedDateTimeEntity? retrievedMax;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMax = await session.GetAsync<ZonedDateTimeEntity>(maxId);
		}

		retrievedMax.Nullable.Should().Be(max);
	}
}
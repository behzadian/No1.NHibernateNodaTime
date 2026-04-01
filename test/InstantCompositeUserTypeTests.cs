using FluentAssertions;
using NHibernate;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Tests for InstantCompositeUserType that stores Instant in two columns
/// </summary>
public class InstantCompositeUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistInstantInTwoColumns()
	{
		// Arrange
		var instant = Instant.FromUtc(2024, 12, 25, 10, 30, 45);
		var entity = new Event() { Name = "Test Event", JustInstant = instant, NullableInstant = instant };

		// Act - Save
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		// Act - Verify in database (check two columns were created)
		using (var session = _sessionFactory.OpenSession())
		{
			var sql = @"
				SELECT JustInstant_Timestamp, JustInstant_Nanoseconds 
				FROM ""Event""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var ts = Convert.ToDateTime(result[0]);
			var ns = Convert.ToInt32(result[1]);

			// Assert - Verify raw column values
			ts.Should().Be(instant.ToDateTimeUtc());
			ns.Should().Be(instant.Nanoseconds());
		}

		// Act - Retrieve via NHibernate
		Event? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<Event>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.JustInstant.Should().Be(instant);
	}

	[Fact]
	public async Task ShouldPreserveNanosecondPrecision()
	{
		// Arrange - Create instant with specific nanosecond value
		var instant = Instant
			.FromUnixTimeSeconds(1609459200) // 2021-01-01 00:00:00
			.PlusNanoseconds(123456789);

		var entity = new Event() { Name = "Precision Test", JustInstant = instant, NullableInstant = instant };

		// Act
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		Event? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<Event>(savedId);
		}

		// Assert
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.JustInstant.Should().Be(instant);
		retrievedEvent.JustInstant.Nanoseconds().Should().Be(123456789);
	}

	[Fact]
	public async Task ShouldHandleNullableInstant()
	{
		// Arrange
		var now = SystemClock.Instance.GetCurrentInstant();
		var entity = new Event() { Name = "Test", JustInstant = now, NullableInstant = null };

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
				SELECT NullableInstant_Timestamp, NullableInstant_Nanoseconds 
				FROM ""Event""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			result[0].Should().BeNull();
			result[1].Should().BeNull();
		}
	}

	[Fact]
	public async Task ShouldHandleMinInstants()
	{
		// Arrange
		var minInstant = Instant.FromDateTimeUtc(new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Utc));

		var minEntity = new Event() { Name = "Min", JustInstant = minInstant, NullableInstant = minInstant };

		// Act
		int minId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			minId = (int)await session.SaveAsync(minEntity);
			await transaction.CommitAsync();
		}

		// Assert
		Event? retrievedMin;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMin = await session.GetAsync<Event>(minId);
		}

		retrievedMin!.JustInstant.Should().Be(minInstant);
	}

	[Fact]
	public async Task ShouldHandleMaxInstant()
	{
		// Arrange
		var maxInstant = Instant.FromDateTimeUtc(new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc));

		var maxEntity = new Event() { Name = "Max", JustInstant = maxInstant, NullableInstant = maxInstant };

		// Act
		int maxId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			maxId = (int)await session.SaveAsync(maxEntity);
			await transaction.CommitAsync();
		}

		// Assert
		Event? retrievedMax;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMax = await session.GetAsync<Event>(maxId);
		}

		retrievedMax!.JustInstant.Should().Be(maxInstant);
	}
}

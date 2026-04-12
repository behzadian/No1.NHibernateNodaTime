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
public class DurationCompositeUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistDuratrionIn2Columns()
	{
		// Arrange
		var duration1 = Duration.FromHours(1.5);
		var duration2 = Duration.FromTicks(360000001);
		var entity = new Event() { Name = "Test Event", DurationValauable = duration1, DurationNullable = duration2 };

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
				SELECT DurationValauable_Seconds, DurationValauable_Nanos, DurationNullable_Seconds, DurationNullable_Nanos
				FROM ""Event""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			// Assert - Verify raw column values
			Convert.ToInt64(result[0]).Should().Be((long)(duration1.ToInt128Nanoseconds() / 1_000_000_000L));
			Convert.ToInt32(result[1]).Should().Be(0);
			Convert.ToInt64(result[2]).Should().Be((long)(duration2.ToInt128Nanoseconds() / 1_000_000_000L));
			Convert.ToInt32(result[3]).Should().Be(100);
		}

		// Act - Retrieve via NHibernate
		Event? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<Event>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent.DurationValauable.Should().Be(duration1);
		retrievedEvent.DurationNullable.Should().Be(duration2);
	}

	[Fact]
	public async Task ShouldHandleNullable()
	{
		// Arrange
		var duration = Duration.FromMinutes(67);
		var entity = new Event() { Name = "Test", DurationValauable = duration, DurationNullable = null };

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
				SELECT DurationValauable_Seconds, DurationValauable_Nanos, DurationNullable_Seconds, DurationNullable_Nanos
				FROM ""Event""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			result[0].Should().NotBeNull();
			result[1].Should().NotBeNull();
			result[2].Should().BeNull();
			result[3].Should().BeNull();
		}
	}

	[Fact]
	public async Task ShouldHandleMin()
	{
		// Arrange
		var min = Duration.MinValue;
		var minEntity = new Event() { Name = "Min", DurationNullable = min };

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

		retrievedMin.DurationNullable.Should().Be(min);
	}

	[Fact]
	public async Task ShouldHandleMax()
	{
		// Arrange
		var max = Duration.MaxValue;
		var maxEntity = new Event() { Name = "Max", DurationNullable = max };

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

		retrievedMax.DurationNullable.Should().Be(max);
	}
}

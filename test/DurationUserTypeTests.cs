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
		var entity = new DurationEntity() { Name = "Test Event", Valauable = duration1, Nullable = duration2 };

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
				SELECT ValauableSeconds, ValauableNanos, NullableSeconds, NullableNanos
				FROM ""DurationEntity""
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
		DurationEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<DurationEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent.Valauable.Should().Be(duration1);
		retrievedEvent.Nullable.Should().Be(duration2);
	}

	[Fact]
	public async Task ShouldHandleNullable()
	{
		// Arrange
		var duration = Duration.FromMinutes(67);
		var entity = new DurationEntity() { Name = "Test", Valauable = duration, Nullable = null };

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
				SELECT ValauableSeconds, ValauableNanos, NullableSeconds, NullableNanos
				FROM ""DurationEntity""
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
		var minEntity = new DurationEntity() { Name = "Min", Nullable = min };

		// Act
		int minId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			minId = (int)await session.SaveAsync(minEntity);
			await transaction.CommitAsync();
		}

		// Assert
		DurationEntity? retrievedMin;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMin = await session.GetAsync<DurationEntity>(minId);
		}

		retrievedMin.Nullable.Should().Be(min);
	}

	[Fact]
	public async Task ShouldHandleMax()
	{
		// Arrange
		var max = Duration.MaxValue;
		var maxEntity = new DurationEntity() { Name = "Max", Nullable = max };

		// Act
		int maxId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			maxId = (int)await session.SaveAsync(maxEntity);
			await transaction.CommitAsync();
		}

		// Assert
		DurationEntity? retrievedMax;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMax = await session.GetAsync<DurationEntity>(maxId);
		}

		retrievedMax.Nullable.Should().Be(max);
	}
}
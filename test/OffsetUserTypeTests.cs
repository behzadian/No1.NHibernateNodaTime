using FluentAssertions;
using NHibernate;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using NodaTime.Calendars;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Tests for InstantCompositeUserType that stores Instant in two columns
/// </summary>
public class OffsetUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistIn1Column()
	{
		// Arrange
		var val = Offset.FromHours(1);
		var entity = new OffsetEntity() { Valauable = val };

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
				SELECT Valauable, ID
				FROM ""OffsetEntity""
				WHERE id = :id";

			var result = await session
				.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var nanos = Convert.ToInt64(result[0]);

			// Assert - Verify raw column values
			nanos.Should().Be((long)TimeSpan.FromHours(1).TotalNanoseconds);
		}

		// Act - Retrieve via NHibernate
		OffsetEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<OffsetEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent.Valauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldPreserveNanoseconds()
	{
		var val = LocalTime.FromHourMinuteSecondNanosecond(1, 2, 3, 4);
		var entity = new LocalTimeEntity() { Name = "Precision Test", LtValauable = val };

		// Act
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		LocalTimeEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<LocalTimeEntity>(savedId);
		}

		// Assert
		retrievedEvent.Should().NotBeNull();
		retrievedEvent.LtValauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable()
	{
		// Arrange
		var val = LocalTime.FromHourMinuteSecondNanosecond(1, 2, 3, 4);
		var entity = new LocalTimeEntity() { Name = "Test", LtNullable = null };

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
				SELECT LtNullable, Id
				FROM ""LocalTimeEntity""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			result[0].Should().BeNull();
		}
	}

	[Fact]
	public async Task ShouldHandleMin()
	{
		// Arrange
		var min = LocalTime.MinValue;
		var minEntity = new LocalTimeEntity() { Name = "Min", LtNullable = min };

		// Act
		int minId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			minId = (int)await session.SaveAsync(minEntity);
			await transaction.CommitAsync();
		}

		// Assert
		LocalTimeEntity? retrievedMin;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMin = await session.GetAsync<LocalTimeEntity>(minId);
		}

		retrievedMin.LtNullable.Should().Be(min);
	}

	[Fact]
	public async Task ShouldHandleMax()
	{
		// Arrange
		var max = LocalTime.MaxValue;
		var maxEntity = new LocalTimeEntity() { Name = "Max", LtNullable = max };

		// Act
		int maxId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			maxId = (int)await session.SaveAsync(maxEntity);
			await transaction.CommitAsync();
		}

		// Assert
		LocalTimeEntity? retrievedMax;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMax = await session.GetAsync<LocalTimeEntity>(maxId);
		}

		retrievedMax.LtNullable.Should().Be(max);
	}
}

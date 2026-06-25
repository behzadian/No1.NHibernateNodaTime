using FluentAssertions;
using NHibernate;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.Model;
using NodaTime;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Tests for InstantCompositeUserType that stores Instant in two columns
/// </summary>
public class InstantCompleteUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistInstantInTwoColumns() {
		// Arrange
		var instant = Instant.FromUtc(2024, 12, 25, 10, 30, 45);
		var entity = new InstantCompleteEntity() { Name = "Test Event", Valuable = instant, Nullable = instant };

		// Act - Save
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		// Act - Verify in database (check two columns were created)
		using (var session = _sessionFactory.OpenSession()) {
			var sql = @"
				SELECT Valuable_Seconds, Valuable_Nanoseconds 
				FROM ""instant_completes""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var ts = Convert.ToInt64(result[0]);
			var ns = Convert.ToInt32(result[1]);

			// Assert - Verify raw column values
			ts.Should().Be(instant.ToUnixTimeSeconds());
			ns.Should().Be(instant.OnlyNanoseconds());
		}

		// Act - Retrieve via NHibernate
		InstantCompleteEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedEvent = await session.GetAsync<InstantCompleteEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.Valuable.Should().Be(instant);
	}

	[Fact]
	public async Task ShouldPreserveNanosecondPrecision() {
		// Arrange - Create instant with specific nanosecond value
		var instant = Instant
			.FromUnixTimeSeconds(1609459200) // 2021-01-01 00:00:00
			.PlusNanoseconds(123456789);

		var entity = new InstantCompleteEntity() { Name = "Precision Test", Valuable = instant, Nullable = instant };

		// Act
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		InstantCompleteEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedEvent = await session.GetAsync<InstantCompleteEntity>(savedId);
		}

		// Assert
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.Valuable.Should().Be(instant);
		retrievedEvent.Valuable.OnlyNanoseconds().Should().Be(123456789);
	}

	[Fact]
	public async Task ShouldHandleNullableInstant() {
		// Arrange
		var now = SystemClock.Instance.GetCurrentInstant();
		var entity = new InstantCompleteEntity() { Name = "Test", Valuable = now, Nullable = null };

		// Act - Save without ModifiedAt
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		// Assert - Both columns should be NULL
		using (var session = _sessionFactory.OpenSession()) {
			var sql = @"
				SELECT Nullable_Seconds, Nullable_Nanoseconds 
				FROM ""instant_completes""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			result[0].Should().BeNull();
			result[1].Should().BeNull();
		}
	}

	[Fact]
	public async Task ShouldHandleMinInstants() {
		// Arrange
		//var minInstant = Instant.FromDateTimeUtc(new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Utc));
		var minInstant = Instant.MinValue;

		var minEntity = new InstantCompleteEntity() { Name = "Min", Valuable = minInstant, Nullable = minInstant };

		// Act
		int minId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			minId = (int)await session.SaveAsync(minEntity);
			await transaction.CommitAsync();
		}

		// Assert
		InstantCompleteEntity? retrievedMin;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedMin = await session.GetAsync<InstantCompleteEntity>(minId);
		}

		retrievedMin!.Valuable.Should().Be(minInstant);
	}

	[Fact]
	public async Task ShouldHandleMaxInstant() {
		// Arrange
		//var maxInstant = Instant.FromDateTimeUtc(new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc));
		var maxInstant = Instant.MaxValue;

		var maxEntity = new InstantCompleteEntity() { Name = "Max", Valuable = maxInstant, Nullable = maxInstant };

		// Act
		int maxId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			maxId = (int)await session.SaveAsync(maxEntity);
			await transaction.CommitAsync();
		}

		// Assert
		InstantCompleteEntity? retrievedMax;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedMax = await session.GetAsync<InstantCompleteEntity>(maxId);
		}

		retrievedMax!.Valuable.Should().Be(maxInstant);
	}
}
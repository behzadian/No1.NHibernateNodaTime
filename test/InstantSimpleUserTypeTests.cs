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
public class InstantCompactUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistInstantInTwoColumns() {
		// Arrange
		var instant = Instant.FromUtc(2024, 12, 25, 10, 30, 45);
		var entity = new InstantCompactEntity() { Valuable = instant, Nullable = instant };

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
				SELECT Valuable, ID
				FROM ""instant_compacts""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var dateTime = Convert.ToDateTime(result[0]);

			// Assert - Verify raw column values
			dateTime.Should().Be(instant.ToDateTimeUtc());
		}

		// Act - Retrieve via NHibernate
		InstantCompactEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedEvent = await session.GetAsync<InstantCompactEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.Valuable.Should().Be(instant);
	}

	[Fact]
	public async Task ShouldHandleNullableInstant() {
		// Arrange
		var now = SystemClock.Instance.GetCurrentInstant();
		var entity = new InstantCompactEntity() { Valuable = now, Nullable = null };

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
				SELECT Nullable, ID
				FROM ""instant_compacts""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			result[0].Should().BeNull();
			result[1].Should().NotBeNull();
		}
	}
}
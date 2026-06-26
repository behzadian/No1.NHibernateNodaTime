using FluentAssertions;
using NHibernate;
using No1.NHibernateNodaTimeTests.Core;
using No1.NHibernateNodaTimeTests.Model;
using NodaTime;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Tests for InstantCompositeUserType that stores Instant in two columns
/// </summary>
public class LocalDateTimeCompactUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistLocalDateTimeInMultiColumns() {
		// Arrange
		var val = new LocalDateTime(1405, 1, 25, 17, 16, 15, 14, CalendarSystem.PersianSimple);
		var entity = new LocalDateTimeCompactEntity() { Valauable = val };

		// Act - Save
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		// Act - Verify in database (check columns were created)
		using (var session = _sessionFactory.OpenSession()) {
			var sql = @"
				SELECT Valauable_Gregorian, Valauable_Calendar, Valauable_Time_Nanos
				FROM ""local_date_time_compacts""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();


			var counter = 0;
			var date = Convert.ToDateTime(result[counter++]);
			var cal = Convert.ToString(result[counter++]);
			var time = Convert.ToInt64(result[counter++]);

			// Assert - Verify raw column values
			date.Should().Be(val.ToDateTimeUnspecified().Date);
			cal.Should().Be("Persian Simple");
			time.Should().Be((long)TimeSpan.Parse("17:16:15.014").TotalNanoseconds);
		}

		// Act - Retrieve via NHibernate
		LocalDateTimeCompactEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedEvent = await session.GetAsync<LocalDateTimeCompactEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.Valauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable() {
		// Arrange
		var entity = new LocalDateTimeCompactEntity() { Nullable = null };

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
				SELECT Nullable_Gregorian, Nullable_Calendar, Nullable_Time_Nanos
				FROM ""local_date_time_compacts""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			for (int i = 0; i < result.Length; i++) {
				result[i].Should().BeNull();
			}
		}
	}
}
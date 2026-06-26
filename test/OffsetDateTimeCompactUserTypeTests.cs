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
public class OffsetDateTimeCompactUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistOffsetDateTimeInMultiColumns() {
		// Arrange
		var val = new OffsetDateTime(new LocalDateTime(1405, 1, 25, 17, 16, 15, 14, CalendarSystem.PersianSimple), Offset.FromHoursAndMinutes(8, 15));
		var entity = new OffsetDateTimeCompactEntity() { Valauable = val };

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
				SELECT Valauable_Calendar, Valauable_Gregorian, Valauable_Time_Nanos, Valauable_Offset_Nanos
				FROM ""offset_date_time_compacts""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var index = 0;
			var cal = Convert.ToString(result[index++]);
			var date = Convert.ToDateTime(result[index++]);
			var time = Convert.ToInt64(result[index++]);
			var offset = Convert.ToInt64(result[index++]);

			// Assert - Verify raw column values
			cal.Should().Be("Persian Simple");
			date.Should().Be(val.Date.ToDateTimeUnspecified().Date);
			time.Should().Be((long)TimeSpan.Parse("17:16:15.014").TotalNanoseconds);
			offset.Should().Be((long)(8.25 * 3600 * 1_000_000_000L));
		}

		// Act - Retrieve via NHibernate
		OffsetDateTimeCompactEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedEvent = await session.GetAsync<OffsetDateTimeCompactEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent.Valauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable() {
		// Arrange
		var entity = new OffsetDateTimeCompactEntity() { Nullable = null };

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
				SELECT Nullable_Calendar, Nullable_Gregorian, Nullable_Time_Nanos, Nullable_Offset_Nanos
				FROM ""offset_date_time_compacts""
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
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
public class OffsetDateCompactUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistOffsetDateInMultiColumns() {
		// Arrange
		var val = new OffsetDate(new LocalDate(1405, 1, 25, CalendarSystem.PersianSimple), Offset.FromNanoseconds(123_456_789L));
		var entity = new OffsetDateCompactEntity() { Valauable = val };

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
				SELECT Valauable_Calendar, Valauable_Gregorian, Valauable_Offset_Nanos
				FROM ""offset_date_compacts""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var index = 0;
			var cal = Convert.ToString(result[index++]);
			var date = Convert.ToDateTime(result[index++]);
			var time = Convert.ToInt64(result[index++]);

			// Assert - Verify raw column values
			date.Should().Be(val.Date.ToDateTimeUnspecified().Date);
			cal.Should().Be("Persian Simple");
			time.Should().Be(Duration.FromNanoseconds(time).NanosecondOfDay);
		}

		// Act - Retrieve via NHibernate
		OffsetDateCompactEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedEvent = await session.GetAsync<OffsetDateCompactEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent.Valauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable() {
		// Arrange
		var entity = new OffsetDateCompactEntity() { Nullable = null };

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
				SELECT Nullable_Gregorian, Nullable_Calendar, Nullable_Offset_Nanos
				FROM ""offset_date_compacts""
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
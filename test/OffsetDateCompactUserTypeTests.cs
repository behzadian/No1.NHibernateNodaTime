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
		var entity = new OffsetDateCompleteEntity() { Valauable = val };

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
				SELECT Valauable_Calendar, Valauable_Era, Valauable_Year, Valauable_Month, Valauable_Day, Valauable_Gregorian, Valauable_Offset_Nanos
				FROM ""offset_date_completes""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var index = 0;
			var cal = Convert.ToString(result[index++]);
			var era = Convert.ToString(result[index++]);
			var year = Convert.ToInt16(result[index++]);
			var month = Convert.ToInt16(result[index++]);
			var day = Convert.ToInt16(result[index++]);
			var date = Convert.ToDateTime(result[index++]);
			var time = Convert.ToInt64(result[index++]);

			// Assert - Verify raw column values
			date.Should().Be(val.Date.ToDateTimeUnspecified().Date);
			cal.Should().Be("Persian Simple");
			era.Should().Be("AP");
			year.Should().Be(1405);
			month.Should().Be(1);
			day.Should().Be(25);
			time.Should().Be(Duration.FromNanoseconds(time).NanosecondOfDay);
		}

		// Act - Retrieve via NHibernate
		OffsetDateCompleteEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedEvent = await session.GetAsync<OffsetDateCompleteEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent.Valauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable() {
		// Arrange
		var entity = new OffsetDateCompleteEntity() { Nullable = null };

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
				SELECT Nullable_Gregorian, Nullable_Calendar, Nullable_Era, Nullable_Year, Nullable_Month, Nullable_Day
				FROM ""offset_date_completes""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			for (int i = 0; i < result.Length; i++) {
				result[i].Should().BeNull();
			}
		}
	}

	[Fact]
	public async Task ShouldHandleMin() {
		// Arrange
		var min = new OffsetDate(LocalDate.MinIsoValue, Offset.MinValue);
		var minEntity = new OffsetDateCompleteEntity() { Nullable = min };

		// Act
		int minId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			minId = (int)await session.SaveAsync(minEntity);
			await transaction.CommitAsync();
		}

		// Assert
		OffsetDateCompleteEntity? retrievedMin;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedMin = await session.GetAsync<OffsetDateCompleteEntity>(minId);
		}

		retrievedMin.Nullable.Should().Be(min);
	}

	[Fact]
	public async Task ShouldHandleMax() {
		// Arrange
		var max = new OffsetDate(LocalDate.MaxIsoValue, Offset.MaxValue);
		var maxEntity = new OffsetDateCompleteEntity() { Nullable = max };

		// Act
		int maxId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			maxId = (int)await session.SaveAsync(maxEntity);
			await transaction.CommitAsync();
		}

		// Assert
		OffsetDateCompleteEntity? retrievedMax;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedMax = await session.GetAsync<OffsetDateCompleteEntity>(maxId);
		}

		retrievedMax.Nullable.Should().Be(max);
	}
}
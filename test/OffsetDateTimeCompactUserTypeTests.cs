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
		var entity = new OffsetDateTimeCompleteEntity() { Valauable = val };

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
				SELECT Valauable_Calendar, Valauable_Era, Valauable_Year, Valauable_Month, Valauable_Day, Valauable_Gregorian, Valauable_Time_Nanos, Valauable_Offset_Nanos
				FROM ""offset_date_time_completes""
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
			var offset = Convert.ToInt64(result[index++]);

			// Assert - Verify raw column values
			cal.Should().Be("Persian Simple");
			era.Should().Be("AP");
			year.Should().Be(1405);
			month.Should().Be(1);
			day.Should().Be(25);
			date.Should().Be(val.Date.ToDateTimeUnspecified().Date);
			time.Should().Be((long)TimeSpan.Parse("17:16:15.014").TotalNanoseconds);
			offset.Should().Be((long)(8.25 * 3600 * 1_000_000_000L));
		}

		// Act - Retrieve via NHibernate
		OffsetDateTimeCompleteEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedEvent = await session.GetAsync<OffsetDateTimeCompleteEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent.Valauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable() {
		// Arrange
		var entity = new OffsetDateTimeCompleteEntity() { Nullable = null };

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
				SELECT Nullable_Calendar, Nullable_Era, Nullable_Year, Nullable_Month, Nullable_Day, Nullable_Gregorian, Nullable_Time_Nanos, Nullable_Offset_Nanos
				FROM ""offset_date_time_completes""
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
		var minMin = new OffsetDateTime(LocalDateTime.MinIsoValue, Offset.MinValue);
		var minMax = new OffsetDateTime(LocalDateTime.MinIsoValue, Offset.MaxValue);
		var minEntity = new OffsetDateTimeCompleteEntity() { Valauable = minMin, Nullable = minMax, };

		// Act
		int minId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			minId = (int)await session.SaveAsync(minEntity);
			await transaction.CommitAsync();
		}

		// Assert
		OffsetDateTimeCompleteEntity? retrievedMin;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedMin = await session.GetAsync<OffsetDateTimeCompleteEntity>(minId);
		}

		retrievedMin.Valauable.Should().Be(minMin);
		retrievedMin.Nullable.Should().Be(minMax);
	}

	[Fact]
	public async Task ShouldHandleMax() {
		// Arrange
		var maxMin = new OffsetDateTime(LocalDateTime.MaxIsoValue, Offset.MinValue);
		var maxMax = new OffsetDateTime(LocalDateTime.MaxIsoValue, Offset.MaxValue);
		var maxEntity = new OffsetDateTimeCompleteEntity() { Valauable = maxMin, Nullable = maxMax, };

		// Act
		int maxId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			maxId = (int)await session.SaveAsync(maxEntity);
			await transaction.CommitAsync();
		}

		// Assert
		OffsetDateTimeCompleteEntity? retrievedMax;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedMax = await session.GetAsync<OffsetDateTimeCompleteEntity>(maxId);
		}

		retrievedMax.Valauable.Should().Be(maxMin);
		retrievedMax.Nullable.Should().Be(maxMax);
	}
}
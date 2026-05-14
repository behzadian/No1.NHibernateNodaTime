using FluentAssertions;
using NHibernate;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.Model;
using NodaTime;
using NodaTime.Calendars;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Tests for InstantCompositeUserType that stores Instant in two columns
/// </summary>
public class LocalDateTimeCompositeUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistLocalDateTimeInMultiColumns()
	{
		// Arrange
		var val = new LocalDateTime(1405, 1, 25, 17, 16, 15, 14, CalendarSystem.PersianSimple);
		var entity = new LocalDateTimeEntity() { Name = "Test Event", Valauable = val };

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
				SELECT Valauable_Gregorian, Valauable_Calendar, Valauable_Era, Valauable_Year, Valauable_Month, Valauable_Day, Valauable_Time_Nanos
				FROM ""local_date_times""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();


			var counter = 0;
			var date = Convert.ToDateTime(result[counter++]);
			var cal = Convert.ToString(result[counter++]);
			var era = Convert.ToString(result[counter++]);
			var year = Convert.ToInt16(result[counter++]);
			var month = Convert.ToInt16(result[counter++]);
			var day = Convert.ToInt16(result[counter++]);
			var time = Convert.ToInt64(result[counter++]);

			// Assert - Verify raw column values
			date.Should().Be(val.ToDateTimeUnspecified().Date);
			cal.Should().Be("Persian Simple");
			era.Should().Be("AP");
			year.Should().Be(1405);
			month.Should().Be(1);
			day.Should().Be(25);
			time.Should().Be((long)TimeSpan.Parse("17:16:15.014").TotalNanoseconds);
		}

		// Act - Retrieve via NHibernate
		LocalDateTimeEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<LocalDateTimeEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.Valauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable()
	{
		// Arrange
		var entity = new LocalDateTimeEntity() { Name = "Test", Nullable = null };

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
				SELECT Nullable_Gregorian, Nullable_Calendar, Nullable_Era, Nullable_Year, Nullable_Month, Nullable_Day
				FROM ""local_date_times""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			for (int i = 0; i < result.Length; i++)
			{
				result[i].Should().BeNull();
			}
		}
	}

	[Fact]
	public async Task ShouldHandleMin()
	{
		// Arrange
		var min = LocalDateTime.MinIsoValue;
		var minEntity = new LocalDateTimeEntity() { Name = "Min", Nullable = min };

		// Act
		int minId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			minId = (int)await session.SaveAsync(minEntity);
			await transaction.CommitAsync();
		}

		// Assert
		LocalDateTimeEntity? retrievedMin;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMin = await session.GetAsync<LocalDateTimeEntity>(minId);
		}

		retrievedMin.Nullable.Should().Be(min);
	}

	[Fact]
	public async Task ShouldHandleMax()
	{
		// Arrange
		var max = LocalDateTime.MaxIsoValue;
		var maxEntity = new LocalDateTimeEntity() { Name = "Max", Nullable = max };

		// Act
		int maxId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			maxId = (int)await session.SaveAsync(maxEntity);
			await transaction.CommitAsync();
		}

		// Assert
		LocalDateTimeEntity? retrievedMax;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMax = await session.GetAsync<LocalDateTimeEntity>(maxId);
		}

		retrievedMax.Nullable.Should().Be(max);
	}
}
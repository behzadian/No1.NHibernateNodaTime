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
public class LocalDateCompositeUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistLocalDateIn4Columns()
	{
		// Arrange
		var val = new LocalDate(1405, 1, 25, CalendarSystem.PersianSimple);
		var entity = new LocalDateEntity() { Name = "Test Event", Valauable = val };

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
				SELECT Valauable_Gregorian, Valauable_Calendar, Valauable_Era, Valauable_Year, Valauable_Month, Valauable_Day
				FROM ""local_dates""
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

			// Assert - Verify raw column values
			date.Should().Be(val.ToDateTimeUnspecified());
			cal.Should().Be("Persian Simple");
			era.Should().Be("AP");
			year.Should().Be(1405);
			month.Should().Be(1);
			day.Should().Be(25);
		}

		// Act - Retrieve via NHibernate
		LocalDateEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<LocalDateEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.Valauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldPreserveEra()
	{
		var val = new LocalDate(Era.AnnoPersico, 1405, 1, 25, CalendarSystem.PersianArithmetic);
		var entity = new LocalDateEntity() { Name = "Precision Test", Valauable = val };

		// Act
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		LocalDateEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<LocalDateEntity>(savedId);
		}

		// Assert
		retrievedEvent.Should().NotBeNull();
		retrievedEvent.Valauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable()
	{
		// Arrange
		var val = new LocalDate(Era.AnnoPersico, 1405, 1, 25, CalendarSystem.PersianArithmetic);
		var entity = new LocalDateEntity() { Name = "Test", Nullable = null };

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
				FROM ""local_dates""
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
		var min = LocalDate.MinIsoValue;
		var minEntity = new LocalDateEntity() { Name = "Min", Nullable = min };

		// Act
		int minId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			minId = (int)await session.SaveAsync(minEntity);
			await transaction.CommitAsync();
		}

		// Assert
		LocalDateEntity? retrievedMin;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMin = await session.GetAsync<LocalDateEntity>(minId);
		}

		retrievedMin.Nullable.Should().Be(min);
	}

	[Fact]
	public async Task ShouldHandleMax()
	{
		// Arrange
		var max = LocalDate.MaxIsoValue;
		var maxEntity = new LocalDateEntity() { Name = "Max", Nullable = max };

		// Act
		int maxId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			maxId = (int)await session.SaveAsync(maxEntity);
			await transaction.CommitAsync();
		}

		// Assert
		LocalDateEntity? retrievedMax;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMax = await session.GetAsync<LocalDateEntity>(maxId);
		}

		retrievedMax.Nullable.Should().Be(max);
	}
}
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
public class LocalDateCompositeUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistLocalDateIn4Columns()
	{
		// Arrange
		var val = new LocalDate(1405, 1, 25, CalendarSystem.PersianSimple);
		var entity = new LocalDateEntity() { Name = "Test Event", LdValauable = val };

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
				SELECT LdValauable_Gregorian, LdValauable_Calendar, LdValauable_Era, LdValauable_Year, LdValauable_Month, LdValauable_Day
				FROM ""LocalDateEntity""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var date = Convert.ToDateTime(result[0]);
			var cal = Convert.ToString(result[1]);
			var era = Convert.ToString(result[2]);
			var year = Convert.ToInt16(result[3]);
			var month = Convert.ToInt16(result[4]);
			var day = Convert.ToInt16(result[5]);

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
		retrievedEvent!.LdValauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldPreserveEra()
	{
		var val = new LocalDate(Era.AnnoPersico, 1405, 1, 25, CalendarSystem.PersianArithmetic);
		var entity = new LocalDateEntity() { Name = "Precision Test", LdValauable = val };

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
		retrievedEvent.LdValauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable()
	{
		// Arrange
		var val = new LocalDate(Era.AnnoPersico, 1405, 1, 25, CalendarSystem.PersianArithmetic);
		var entity = new LocalDateEntity() { Name = "Test", LdNullable = null };

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
				SELECT LdNullable_Gregorian, LdNullable_Calendar, LdNullable_Era, LdNullable_Year, LdNullable_Month, LdNullable_Day
				FROM ""LocalDateEntity""
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
		var minEntity = new LocalDateEntity() { Name = "Min", LdNullable = min };

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

		retrievedMin.LdNullable.Should().Be(min);
	}

	[Fact]
	public async Task ShouldHandleMax()
	{
		// Arrange
		var max = LocalDate.MaxIsoValue;
		var maxEntity = new LocalDateEntity() { Name = "Max", LdNullable = max };

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

		retrievedMax.LdNullable.Should().Be(max);
	}
}

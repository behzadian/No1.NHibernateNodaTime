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
public class LocalDateTimeCompositeUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistLocalDateTimeInMultiColumns()
	{
		// Arrange
		var val = new LocalDateTime(1405, 1, 25, 17, 16, 15, 14, CalendarSystem.PersianSimple);
		var entity = new LocalDateTimeEntity() { Name = "Test Event", LdtValauable = val };

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
				SELECT LdtValauable_Gregorian, LdtValauable_Calendar, LdtValauable_Year, LdtValauable_Month, LdtValauable_Day, LdtValauable_Nanos
				FROM ""LocalDateTimeEntity""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var date = Convert.ToDateTime(result[0]);
			var cal = Convert.ToString(result[1]);
			var year = Convert.ToInt16(result[2]);
			var month = Convert.ToInt16(result[3]);
			var day = Convert.ToInt16(result[4]);
			var time = Convert.ToInt64(result[5]);

			// Assert - Verify raw column values
			date.Should().Be(val.ToDateTimeUnspecified().Date);
			cal.Should().Be("Persian Simple");
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
		retrievedEvent!.LdtValauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable()
	{
		// Arrange
		var entity = new LocalDateTimeEntity() { Name = "Test", LdtNullable = null };

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
				SELECT LdtNullable_Gregorian, LdtNullable_Calendar, LdtNullable_Era, LdtNullable_Year, LdtNullable_Month, LdtNullable_Day
				FROM ""LocalDateTimeEntity""
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
		var minEntity = new LocalDateTimeEntity() { Name = "Min", LdtNullable = min };

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

		retrievedMin.LdtNullable.Should().Be(min);
	}

	[Fact]
	public async Task ShouldHandleMax()
	{
		// Arrange
		var max = LocalDateTime.MaxIsoValue;
		var maxEntity = new LocalDateTimeEntity() { Name = "Max", LdtNullable = max };

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

		retrievedMax.LdtNullable.Should().Be(max);
	}
}

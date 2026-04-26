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
		var entity = new Event() { Name = "Test Event", LdtValauable = val };

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
				FROM ""Event""
				WHERE id = :id";

			var command = session.Connection.CreateCommand();//.ExecuteReader();
			command.CommandText = sql.Replace(":id", "1");
			var reader = command.ExecuteReader();
			string log = "";
			while (reader.Read())
			{
				for (int i = 0; i < reader.FieldCount; i++)
				{
					log += reader.GetValue(i).ToString() + "|";
				}
			}
			reader.Close();

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
		Event? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<Event>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.LdtValauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldPreserveEra()
	{
		var val = new LocalDate(Era.AnnoPersico, 1405, 1, 25, CalendarSystem.PersianArithmetic);
		var entity = new Event() { Name = "Precision Test", LdValauable = val };

		// Act
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		Event? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<Event>(savedId);
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
		var entity = new Event() { Name = "Test", LdNullable = null };

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
				FROM ""Event""
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
		var minEntity = new Event() { Name = "Min", LdNullable = min };

		// Act
		int minId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			minId = (int)await session.SaveAsync(minEntity);
			await transaction.CommitAsync();
		}

		// Assert
		Event? retrievedMin;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMin = await session.GetAsync<Event>(minId);
		}

		retrievedMin.LdNullable.Should().Be(min);
	}

	[Fact]
	public async Task ShouldHandleMax()
	{
		// Arrange
		var max = LocalDate.MaxIsoValue;
		var maxEntity = new Event() { Name = "Max", LdNullable = max };

		// Act
		int maxId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction())
		{
			maxId = (int)await session.SaveAsync(maxEntity);
			await transaction.CommitAsync();
		}

		// Assert
		Event? retrievedMax;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedMax = await session.GetAsync<Event>(maxId);
		}

		retrievedMax.LdNullable.Should().Be(max);
	}
}

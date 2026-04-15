using FluentAssertions;
using NHibernate;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
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
		var entity = new Event() { Name = "Test Event", LdValauable = val };

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
				SELECT LdValauable_Gregorian, LdValauable_Calendar, LdValauable_Era, LdValauable_YearOfEra
				FROM ""Event""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var date = Convert.ToDateTime(result[0]);
			var cal = Convert.ToString(result[1]);
			var era = Convert.ToString(result[2]);
			var yearOfEra = Convert.ToInt16(result[3]);

			// Assert - Verify raw column values
			date.Should().Be(val.ToDateTimeUnspecified());
			cal.Should().Be("Persian Simple");
			era.Should().Be("AP");
			yearOfEra.Should().Be(1405);
		}

		// Act - Retrieve via NHibernate
		Event? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<Event>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.LdValauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldPreserveNanosecondPrecision()
	{
		var zdt = Instant
			.FromUnixTimeSeconds(1609459200) // 2021-01-01 00:00:00
			.PlusNanoseconds(123456789)
			.InUtc();

		var entity = new Event() { Name = "Precision Test", ZdtValauable = zdt };

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
		retrievedEvent!.ZdtValauable.Should().Be(zdt);
		retrievedEvent.ZdtValauable.ToInstant().OnlyNanoseconds().Should().Be(123456789);
	}

	[Fact]
	public async Task ShouldHandleNullable()
	{
		// Arrange
		var now = SystemClock.Instance.GetCurrentInstant().InUtc();
		var entity = new Event() { Name = "Test", ZdtValauable = now, ZdtNullable = null };

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
				SELECT ZdtNullable_Seconds, ZdtNullable_Nanoseconds, ZdtNullable_ZoneID, ZdtNullable_UTC, ZdtNullable_Local
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
		var min = Instant.MinValue.InUtc();

		var minEntity = new Event() { Name = "Min", ZdtNullable = min };

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

		retrievedMin!.ZdtNullable.Should().Be(min);
	}

	[Fact]
	public async Task ShouldHandleMax()
	{
		// Arrange
		var max = Instant.MaxValue.InUtc();

		var maxEntity = new Event() { Name = "Max", ZdtNullable = max };

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

		retrievedMax.ZdtNullable.Should().Be(max);
	}
}

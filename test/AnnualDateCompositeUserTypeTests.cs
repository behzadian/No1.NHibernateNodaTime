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
public class AnnualDateCompositeUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistIn2Columns()
	{
		// Arrange
		var val = new AnnualDate(11, 27);
		var entity = new AnnualDateEntity() { Name = "Test Event", AnnualDateValauable = val };

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
				SELECT AnnualDateValauable_Month, AnnualDateValauable_Day
				FROM ""AnnualDateEntity""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var month = Convert.ToInt16(result[0]);
			var day = Convert.ToInt16(result[1]);

			// Assert - Verify raw column values
			month.Should().Be((short)val.Month);
			day.Should().Be((short)val.Day);
		}

		// Act - Retrieve via NHibernate
		AnnualDateEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<AnnualDateEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.AnnualDateValauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable()
	{
		// Arrange
		var entity = new AnnualDateEntity() { Name = "Test Event", AnnualDateNullable = null };

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
				SELECT AnnualDateNullable_Month, AnnualDateNullable_Day
				FROM ""AnnualDateEntity""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			for (int i = 0; i < result.Length; i++)
			{
				result[i].Should().BeNull();
			}
		}

		// Act - Retrieve via NHibernate
		AnnualDateEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession())
		{
			retrievedEvent = await session.GetAsync<AnnualDateEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent!.AnnualDateNullable.Should().BeNull();
	}
}

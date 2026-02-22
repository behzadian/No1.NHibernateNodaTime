using FluentAssertions;
using NHibernate;
using No1.NHibernateNodaTimeTests.TestEntities;
using NodaTime;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Tests for InstantCompositeUserType that stores Instant in two columns
/// </summary>
public class InstantCompositeUserTypeTests : IClassFixture<NHibernateCompositeTestFixture>
{
    private readonly ISessionFactory _sessionFactory;

    public InstantCompositeUserTypeTests(NHibernateCompositeTestFixture fixture)
    {
        _sessionFactory = fixture.SessionFactory;
    }

    [Fact]
    public async Task ShouldPersistInstantInTwoColumns()
    {
        // Arrange
        var instant = Instant.FromUtc(2024, 12, 25, 10, 30, 45);
        var entity = new Event() { Name = "Test Event", JustInstant = instant, NullableInstant = instant };

        // Act - Save
        int savedId;
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            savedId = (int)await session.SaveAsync(entity);
            await transaction.CommitAsync();
        }

        // Act - Verify in database (check two columns were created)
        using (var session = _sessionFactory.OpenSession())
        {
            var sql = @"
                SELECT created_at_seconds, created_at_nanoseconds 
                FROM events 
                WHERE id = :id";

            var result = await session.CreateSQLQuery(sql)
                .SetParameter("id", savedId)
                .UniqueResultAsync<object[]>();

            var seconds = Convert.ToInt64(result[0]);
            var nanoseconds = Convert.ToInt32(result[1]);

            // Assert - Verify raw column values
            seconds.Should().Be(instant.ToUnixTimeSeconds());
            //nanoseconds.Should().Be(instant.NanosecondOfSecond);
        }

        // Act - Retrieve via NHibernate
        Event? retrievedEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedEvent = await session.GetAsync<Event>(savedId);
        }

        // Assert - Verify object reconstruction
        retrievedEvent.Should().NotBeNull();
        retrievedEvent!.JustInstant.Should().Be(instant);
    }

    [Fact]
    public async Task ShouldPreserveNanosecondPrecision()
    {
        // Arrange - Create instant with specific nanosecond value
        var instant = Instant.FromUnixTimeSeconds(1609459200) // 2021-01-01 00:00:00
            .PlusNanoseconds(123456789);

        var entity = new Event() { Name = "Precision Test", JustInstant = instant, NullableInstant = instant };

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
        retrievedEvent!.JustInstant.Should().Be(instant);
        retrievedEvent.JustInstant.Nanoseconds().Should().Be(123456789);
    }

    [Fact]
    public async Task ShouldHandleNullableInstant()
    {
        // Arrange
        var now = SystemClock.Instance.GetCurrentInstant();
        var entity = new Event() { Name = "Test", JustInstant = now, NullableInstant = now };

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
                SELECT modified_at_seconds, modified_at_nanoseconds 
                FROM events 
                WHERE id = :id";

            var result = await session.CreateSQLQuery(sql)
                .SetParameter("id", savedId)
                .UniqueResultAsync<object[]>();

            result[0].Should().BeNull();
            result[1].Should().BeNull();
        }
    }

    [Fact]
    public async Task ShouldHandleMinInstants()
    {
        // Arrange
        var minInstant = Instant.MinValue;

        var minEntity = new Event() { Name = "Min", JustInstant = minInstant, NullableInstant = minInstant };

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

        retrievedMin!.JustInstant.Should().Be(minInstant);
    }

    [Fact]
    public async Task ShouldHandleMaxInstant()
    {
        // Arrange
        var maxInstant = Instant.MaxValue;

        var maxEntity = new Event() { Name = "Max", JustInstant = maxInstant, NullableInstant = maxInstant };

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

        retrievedMax!.JustInstant.Should().Be(maxInstant);
    }
}

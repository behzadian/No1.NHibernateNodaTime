using FluentAssertions;
using NHibernate;
using NHibernate.Linq;
using NodaTime;
using No1.NodaTimeNHibernate.Tests.Infrastructure;
using No1.NodaTimeNHibernate.Tests.TestEntities;
using Xunit;

namespace No1.NodaTimeNHibernate.Tests;

/// <summary>
/// Integration tests for NodaTime Instant persistence with NHibernate and PostgreSQL
/// </summary>
public class InstantPersistenceTests : IClassFixture<NHibernateTestFixture>
{
    private readonly ISessionFactory _sessionFactory;

    public InstantPersistenceTests(NHibernateTestFixture fixture)
    {
        _sessionFactory = fixture.SessionFactory;
    }

    [Fact]
    public async Task ShouldPersistAndRetrieveInstant()
    {
        // Arrange
        var now = SystemClock.Instance.GetCurrentInstant();
        var eventDate = Instant.FromUtc(2024, 12, 25, 10, 30, 0);
        var eventEntity = new Event("Christmas Party", now, eventDate);

        int savedId;

        // Act - Save
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            savedId = (int)await session.SaveAsync(eventEntity);
            await transaction.CommitAsync();
        }

        // Act - Retrieve
        Event? retrievedEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedEvent = await session.GetAsync<Event>(savedId);
        }

        // Assert
        retrievedEvent.Should().NotBeNull();
        retrievedEvent!.Name.Should().Be("Christmas Party");
        retrievedEvent.CreatedAt.Should().Be(now);
        retrievedEvent.EventDate.Should().Be(eventDate);
        retrievedEvent.ModifiedAt.Should().BeNull();
    }

    [Fact]
    public async Task ShouldHandleNullableInstant()
    {
        // Arrange
        var now = SystemClock.Instance.GetCurrentInstant();
        var eventDate = Instant.FromUtc(2025, 1, 1, 0, 0, 0);
        var eventEntity = new Event("New Year", now, eventDate);

        int savedId;

        // Act - Save without ModifiedAt
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            savedId = (int)await session.SaveAsync(eventEntity);
            await transaction.CommitAsync();
        }

        // Act - Update with ModifiedAt
        var modifiedTime = now.Plus(Duration.FromHours(2));
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            var loadedEvent = await session.GetAsync<Event>(savedId);
            loadedEvent!.UpdateModifiedAt(modifiedTime);
            await session.UpdateAsync(loadedEvent);
            await transaction.CommitAsync();
        }

        // Act - Retrieve updated
        Event? retrievedEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedEvent = await session.GetAsync<Event>(savedId);
        }

        // Assert
        retrievedEvent.Should().NotBeNull();
        retrievedEvent!.ModifiedAt.Should().NotBeNull();
        retrievedEvent.ModifiedAt.Should().Be(modifiedTime);
    }

    [Fact]
    public async Task ShouldPreservePrecisionWithNanoseconds()
    {
        // Arrange - Create an Instant with nanosecond precision
        var preciseInstant = Instant.FromUnixTimeSeconds(1609459200) // 2021-01-01 00:00:00 UTC
            .PlusNanoseconds(123456789);
        
        var eventEntity = new Event("Precision Test", preciseInstant, preciseInstant);

        int savedId;

        // Act - Save
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            savedId = (int)await session.SaveAsync(eventEntity);
            await transaction.CommitAsync();
        }

        // Act - Retrieve
        Event? retrievedEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedEvent = await session.GetAsync<Event>(savedId);
        }

        // Assert
        retrievedEvent.Should().NotBeNull();
        retrievedEvent!.CreatedAt.Should().Be(preciseInstant);
        retrievedEvent.EventDate.Should().Be(preciseInstant);
    }

    [Fact]
    public async Task ShouldQueryByInstantRange()
    {
        // Arrange
        var baseTime = Instant.FromUtc(2024, 6, 15, 12, 0, 0);
        var events = new[]
        {
            new Event("Event 1", baseTime, baseTime),
            new Event("Event 2", baseTime.Plus(Duration.FromHours(1)), baseTime.Plus(Duration.FromHours(1))),
            new Event("Event 3", baseTime.Plus(Duration.FromHours(2)), baseTime.Plus(Duration.FromHours(2))),
            new Event("Event 4", baseTime.Plus(Duration.FromHours(3)), baseTime.Plus(Duration.FromHours(3)))
        };

        // Act - Save all
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            foreach (var evt in events)
            {
                await session.SaveAsync(evt);
            }
            await transaction.CommitAsync();
        }

        // Act - Query by range
        var startRange = baseTime.Plus(Duration.FromMinutes(30));
        var endRange = baseTime.Plus(Duration.FromHours(2).Plus(Duration.FromMinutes(30)));

        List<Event> queriedEvents;
        using (var session = _sessionFactory.OpenSession())
        {
            queriedEvents = await session.Query<Event>()
                .Where(e => e.EventDate >= startRange && e.EventDate <= endRange)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        // Assert
        queriedEvents.Should().HaveCount(2);
        queriedEvents[0].Name.Should().Be("Event 2");
        queriedEvents[1].Name.Should().Be("Event 3");
    }

    [Fact]
    public async Task ShouldHandleMinAndMaxInstants()
    {
        // Arrange
        var minInstant = Instant.MinValue;
        var maxInstant = Instant.MaxValue;
        
        var minEvent = new Event("Min Event", minInstant, minInstant);
        var maxEvent = new Event("Max Event", maxInstant, maxInstant);

        int minId, maxId;

        // Act - Save
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            minId = (int)await session.SaveAsync(minEvent);
            maxId = (int)await session.SaveAsync(maxEvent);
            await transaction.CommitAsync();
        }

        // Act - Retrieve
        Event? retrievedMinEvent, retrievedMaxEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedMinEvent = await session.GetAsync<Event>(minId);
            retrievedMaxEvent = await session.GetAsync<Event>(maxId);
        }

        // Assert
        retrievedMinEvent.Should().NotBeNull();
        retrievedMinEvent!.CreatedAt.Should().Be(minInstant);
        
        retrievedMaxEvent.Should().NotBeNull();
        retrievedMaxEvent!.CreatedAt.Should().Be(maxInstant);
    }

    [Fact]
    public async Task ShouldUpdateInstantValues()
    {
        // Arrange
        var originalTime = Instant.FromUtc(2024, 1, 1, 0, 0, 0);
        var updatedTime = Instant.FromUtc(2024, 12, 31, 23, 59, 59);
        var eventEntity = new Event("Update Test", originalTime, originalTime);

        int savedId;

        // Act - Save
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            savedId = (int)await session.SaveAsync(eventEntity);
            await transaction.CommitAsync();
        }

        // Act - Update
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            var loadedEvent = await session.GetAsync<Event>(savedId);
            loadedEvent!.UpdateModifiedAt(updatedTime);
            await session.UpdateAsync(loadedEvent);
            await transaction.CommitAsync();
        }

        // Act - Retrieve
        Event? retrievedEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedEvent = await session.GetAsync<Event>(savedId);
        }

        // Assert
        retrievedEvent.Should().NotBeNull();
        retrievedEvent!.CreatedAt.Should().Be(originalTime);
        retrievedEvent.ModifiedAt.Should().Be(updatedTime);
    }

    [Fact]
    public async Task ShouldHandleMultipleConcurrentSessions()
    {
        // Arrange
        var time1 = Instant.FromUtc(2024, 3, 15, 10, 0, 0);
        var time2 = Instant.FromUtc(2024, 3, 15, 11, 0, 0);

        // Act - Save from different sessions concurrently
        var task1 = Task.Run(async () =>
        {
            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            var evt = new Event("Concurrent 1", time1, time1);
            var id = await session.SaveAsync(evt);
            await transaction.CommitAsync();
            return (int)id;
        });

        var task2 = Task.Run(async () =>
        {
            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            var evt = new Event("Concurrent 2", time2, time2);
            var id = await session.SaveAsync(evt);
            await transaction.CommitAsync();
            return (int)id;
        });

        var ids = await Task.WhenAll(task1, task2);

        // Act - Retrieve
        using var verifySession = _sessionFactory.OpenSession();
        var event1 = await verifySession.GetAsync<Event>(ids[0]);
        var event2 = await verifySession.GetAsync<Event>(ids[1]);

        // Assert
        event1.Should().NotBeNull();
        event1!.CreatedAt.Should().Be(time1);
        
        event2.Should().NotBeNull();
        event2!.CreatedAt.Should().Be(time2);
    }
}

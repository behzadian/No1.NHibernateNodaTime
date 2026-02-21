using FluentAssertions;
using NHibernate;
using NHibernate.Linq;
using NodaTime;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Additional edge case tests for Instant persistence
/// </summary>
public class InstantEdgeCaseTests(NHibernateTestFixture fixture) : IClassFixture<NHibernateTestFixture>
{
    private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

    [Fact]
    public async Task ShouldHandleInstantAtUnixEpoch()
    {
        // Arrange
        var unixEpoch = Instant.FromUnixTimeSeconds(0);
        var eventEntity = new Event("Unix Epoch Event", unixEpoch, unixEpoch);

        // Act
        int savedId;
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            savedId = (int)await session.SaveAsync(eventEntity);
            await transaction.CommitAsync();
        }

        Event? retrievedEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedEvent = await session.GetAsync<Event>(savedId);
        }

        // Assert
        retrievedEvent.Should().NotBeNull();
        retrievedEvent!.CreatedAt.Should().Be(unixEpoch);
        retrievedEvent.EventDate.Should().Be(unixEpoch);
    }

    [Fact]
    public async Task ShouldHandleInstantBeforeUnixEpoch()
    {
        // Arrange - January 1, 1960
        var preEpoch = Instant.FromUtc(1960, 1, 1, 0, 0, 0);
        var eventEntity = new Event("Pre-Unix Epoch", preEpoch, preEpoch);

        // Act
        int savedId;
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            savedId = (int)await session.SaveAsync(eventEntity);
            await transaction.CommitAsync();
        }

        Event? retrievedEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedEvent = await session.GetAsync<Event>(savedId);
        }

        // Assert
        retrievedEvent.Should().NotBeNull();
        retrievedEvent!.CreatedAt.Should().Be(preEpoch);
    }

    [Fact]
    public async Task ShouldHandleFarFutureInstant()
    {
        // Arrange - January 1, 2100
        var futureInstant = Instant.FromUtc(2100, 1, 1, 0, 0, 0);
        var eventEntity = new Event("Far Future", futureInstant, futureInstant);

        // Act
        int savedId;
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            savedId = (int)await session.SaveAsync(eventEntity);
            await transaction.CommitAsync();
        }

        Event? retrievedEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedEvent = await session.GetAsync<Event>(savedId);
        }

        // Assert
        retrievedEvent.Should().NotBeNull();
        retrievedEvent!.CreatedAt.Should().Be(futureInstant);
    }

    [Fact]
    public async Task ShouldHandleMicrosecondPrecision()
    {
        // Arrange
        var baseInstant = Instant.FromUtc(2024, 6, 15, 14, 30, 25);
        var microInstant = baseInstant.PlusNanoseconds(123456000); // 123.456 milliseconds
        var eventEntity = new Event("Microsecond Test", microInstant, microInstant);

        // Act
        int savedId;
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            savedId = (int)await session.SaveAsync(eventEntity);
            await transaction.CommitAsync();
        }

        Event? retrievedEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedEvent = await session.GetAsync<Event>(savedId);
        }

        // Assert
        retrievedEvent.Should().NotBeNull();
        retrievedEvent!.CreatedAt.Should().Be(microInstant);
    }

    [Fact]
    public async Task ShouldMaintainInstantOrderingInQueries()
    {
        // Arrange
        var baseTime = Instant.FromUtc(2024, 1, 1, 0, 0, 0);
        var instants = Enumerable.Range(0, 10)
            .Select(i => baseTime.Plus(Duration.FromHours(i)))
            .ToList();

        // Shuffle for insertion
        var shuffled = instants.OrderBy(_ => Guid.NewGuid()).ToList();

        // Act - Save in random order
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            foreach (var instant in shuffled)
            {
                var evt = new Event($"Event {instant.ToUnixTimeSeconds()}", instant, instant);
                await session.SaveAsync(evt);
            }
            await transaction.CommitAsync();
        }

        // Act - Query ordered by EventDate
        List<Event> orderedEvents;
        using (var session = _sessionFactory.OpenSession())
        {
            orderedEvents = await session.Query<Event>()
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        // Assert
        orderedEvents.Should().HaveCountGreaterOrEqualTo(10);
        var retrieved = orderedEvents
            .Where(e => e.Name.StartsWith("Event"))
            .Select(e => e.EventDate)
            .ToList();

        retrieved.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task ShouldHandleLeapSecondAdjacent()
    {
        // Arrange - Near a leap second boundary (e.g., June 30, 2015 23:59:60)
        // NodaTime handles this internally, but we test persistence around these times
        var nearLeapSecond = Instant.FromUtc(2015, 6, 30, 23, 59, 59);
        var eventEntity = new Event("Leap Second Test", nearLeapSecond, nearLeapSecond);

        // Act
        int savedId;
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            savedId = (int)await session.SaveAsync(eventEntity);
            await transaction.CommitAsync();
        }

        Event? retrievedEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedEvent = await session.GetAsync<Event>(savedId);
        }

        // Assert
        retrievedEvent.Should().NotBeNull();
        retrievedEvent!.CreatedAt.Should().Be(nearLeapSecond);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999999)]
    public async Task ShouldHandleVariousNanosecondValues(long nanoseconds)
    {
        // Arrange
        var baseInstant = Instant.FromUtc(2024, 3, 15, 10, 30, 0);
        var instant = baseInstant.PlusNanoseconds(nanoseconds);
        var eventEntity = new Event($"Nano Test {nanoseconds}", instant, instant);

        // Act
        int savedId;
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            savedId = (int)await session.SaveAsync(eventEntity);
            await transaction.CommitAsync();
        }

        Event? retrievedEvent;
        using (var session = _sessionFactory.OpenSession())
        {
            retrievedEvent = await session.GetAsync<Event>(savedId);
        }

        // Assert
        retrievedEvent.Should().NotBeNull();
        retrievedEvent!.CreatedAt.Should().Be(instant);
    }

    [Fact]
    public async Task ShouldHandleBatchInserts()
    {
        // Arrange
        const int batchSize = 100;
        var baseTime = Instant.FromUtc(2024, 1, 1, 0, 0, 0);
        var events = Enumerable.Range(0, batchSize)
            .Select(i => new Event(
                $"Batch Event {i}",
                baseTime.Plus(Duration.FromSeconds(i)),
                baseTime.Plus(Duration.FromSeconds(i))))
            .ToList();

        // Act - Batch insert
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            foreach (var evt in events)
            {
                await session.SaveAsync(evt);
            }
            await transaction.CommitAsync();
        }

        // Act - Count
        int count;
        using (var session = _sessionFactory.OpenSession())
        {
            count = await session.Query<Event>()
                .Where(e => e.Name.StartsWith("Batch Event"))
                .CountAsync();
        }

        // Assert
        count.Should().Be(batchSize);
    }

    [Fact]
    public async Task ShouldHandleInstantComparisonsInWhereClause()
    {
        // Arrange
        var now = SystemClock.Instance.GetCurrentInstant();
        var past = now.Minus(Duration.FromDays(1));
        var future = now.Plus(Duration.FromDays(1));

        var events = new[]
        {
            new Event("Past Event", past, past),
            new Event("Current Event", now, now),
            new Event("Future Event", future, future)
        };

        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            foreach (var evt in events)
            {
                await session.SaveAsync(evt);
            }
            await transaction.CommitAsync();
        }

        // Act - Test different comparison operators
        List<Event> beforeNow, afterNow, exactlyNow;
        using (var session = _sessionFactory.OpenSession())
        {
            beforeNow = await session.Query<Event>()
                .Where(e => e.EventDate < now)
                .ToListAsync();

            afterNow = await session.Query<Event>()
                .Where(e => e.EventDate > now)
                .ToListAsync();

            exactlyNow = await session.Query<Event>()
                .Where(e => e.EventDate == now)
                .ToListAsync();
        }

        // Assert
        beforeNow.Should().Contain(e => e.Name == "Past Event");
        afterNow.Should().Contain(e => e.Name == "Future Event");
        exactlyNow.Should().Contain(e => e.Name == "Current Event");
    }
}

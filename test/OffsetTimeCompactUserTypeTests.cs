using FluentAssertions;
using NHibernate;
using No1.NHibernateNodaTimeTests.Core;
using No1.NHibernateNodaTimeTests.Model;
using NodaTime;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Tests for InstantCompositeUserType that stores Instant in two columns
/// </summary>
public class OffsetTimeCompactUserTypeTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task ShouldPersistInMultiColumns() {
		// Arrange
		var val = new OffsetTime(new LocalTime(17, 16, 15, 14), Offset.FromHoursAndMinutes(8, 15));
		var entity = new OffsetTimeEntity() { Valauable = val };

		// Act - Save
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		// Act - Verify in database (check columns were created)
		using (var session = _sessionFactory.OpenSession()) {
			var sql = @"
				SELECT Valauable_Time_Nanos, Valauable_Offset_Nanos
				FROM ""offset_times""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			var index = 0;
			var time = Convert.ToInt64(result[index++]);
			var offset = Convert.ToInt64(result[index++]);

			// Assert - Verify raw column values
			time.Should().Be((long)TimeSpan.Parse("17:16:15.014").TotalNanoseconds);
			offset.Should().Be((long)(8.25 * 3600 * 1_000_000_000L));
		}

		// Act - Retrieve via NHibernate
		OffsetTimeEntity? retrievedEvent;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedEvent = await session.GetAsync<OffsetTimeEntity>(savedId);
		}

		// Assert - Verify object reconstruction
		retrievedEvent.Should().NotBeNull();
		retrievedEvent.Valauable.Should().Be(val);
	}

	[Fact]
	public async Task ShouldHandleNullable() {
		// Arrange
		var entity = new OffsetTimeEntity() { Nullable = null };

		// Act - Save without ModifiedAt
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		// Assert - Both columns should be NULL
		using (var session = _sessionFactory.OpenSession()) {
			var sql = @"
				SELECT Nullable_Time_Nanos, Nullable_Offset_Nanos
				FROM ""offset_times""
				WHERE id = :id";

			var result = await session.CreateSQLQuery(sql)
				.SetParameter("id", savedId)
				.UniqueResultAsync<object[]>();

			for (int i = 0; i < result.Length; i++) {
				result[i].Should().BeNull();
			}
		}
	}

	[Fact]
	public async Task ShouldHandleMin() {
		// Arrange
		var minMin = new OffsetTime(LocalTime.MinValue, Offset.MinValue);
		var minMax = new OffsetTime(LocalTime.MinValue, Offset.MaxValue);
		var minEntity = new OffsetTimeEntity() { Valauable = minMin, Nullable = minMax, };

		// Act
		int minId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			minId = (int)await session.SaveAsync(minEntity);
			await transaction.CommitAsync();
		}

		// Assert
		OffsetTimeEntity? retrievedMin;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedMin = await session.GetAsync<OffsetTimeEntity>(minId);
		}

		retrievedMin.Valauable.Should().Be(minMin);
		retrievedMin.Nullable.Should().Be(minMax);
	}

	[Fact]
	public async Task ShouldHandleMax() {
		// Arrange
		var maxMin = new OffsetTime(LocalTime.MaxValue, Offset.MinValue);
		var maxMax = new OffsetTime(LocalTime.MaxValue, Offset.MaxValue);
		var maxEntity = new OffsetTimeEntity() { Valauable = maxMin, Nullable = maxMax, };

		// Act
		int maxId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			maxId = (int)await session.SaveAsync(maxEntity);
			await transaction.CommitAsync();
		}

		// Assert
		OffsetTimeEntity? retrievedMax;
		using (var session = _sessionFactory.OpenSession()) {
			retrievedMax = await session.GetAsync<OffsetTimeEntity>(maxId);
		}

		retrievedMax.Valauable.Should().Be(maxMin);
		retrievedMax.Nullable.Should().Be(maxMax);
	}
}
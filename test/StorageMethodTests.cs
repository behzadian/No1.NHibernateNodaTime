using FluentAssertions;
using NHibernate;
using No1.NHibernateNodaTimeTests.Core;
using No1.NHibernateNodaTimeTests.Model;
using Renci.SshNet;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

public class StorageMethodTests(NHibernateCompositeTestFixture fixture) : IClassFixture<NHibernateCompositeTestFixture>
{
	private readonly ISessionFactory _sessionFactory = fixture.SessionFactory;

	[Fact]
	public async Task VerifyColumnesForClassSpecifiedCompactStorageMethod() {
		// Arrange
		var entity = new StorageClassSpecifiedCompactTestEntity() { };

		// Act - Save
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		// Act - Verify in database (check columns were created)
		using (var session = _sessionFactory.OpenSession()) {
			var allCols = new string[]{
				"id",
				"complete_duration_seconds",
				"complete_duration_nanos",
				"compact_duration",
				"unspecified_duration",
				"complete_instant_seconds",
				"complete_instant_nanoseconds",
				"complete_instant_timestamp",
				"compact_instant",
				"unspecified_instant",
				"complete_local_date_calendar",
				"complete_local_date_era",
				"complete_local_date_year",
				"complete_local_date_month",
				"complete_local_date_day",
				"complete_local_date_gregorian",
				"compact_local_date_calendar",
				"compact_local_date_gregorian",
				"unspecified_local_date_calendar",
				"unspecified_local_date_gregorian",
				"complete_local_date_time_calendar",
				"complete_local_date_time_era",
				"complete_local_date_time_year",
				"complete_local_date_time_month",
				"complete_local_date_time_day",
				"complete_local_date_time_gregorian",
				"complete_local_date_time_time_nanos",
				"compact_local_date_time_calendar",
				"compact_local_date_time_gregorian",
				"compact_local_date_time_time_nanos",
				"unspecified_local_date_time_calendar",
				"unspecified_local_date_time_gregorian",
				"unspecified_local_date_time_time_nanos",
				"complete_offset_date_calendar",
				"complete_offset_date_era",
				"complete_offset_date_year",
				"complete_offset_date_month",
				"complete_offset_date_day",
				"complete_offset_date_gregorian",
				"complete_offset_date_offset_nanos",
				"compact_offset_date_calendar",
				"compact_offset_date_gregorian",
				"compact_offset_date_offset_nanos",
				"unspecified_offset_date_calendar",
				"unspecified_offset_date_gregorian",
				"unspecified_offset_date_offset_nanos",
				"complete_offset_date_time_calendar",
				"complete_offset_date_time_era",
				"complete_offset_date_time_year",
				"complete_offset_date_time_month",
				"complete_offset_date_time_day",
				"complete_offset_date_time_gregorian",
				"complete_offset_date_time_time_nanos",
				"complete_offset_date_time_offset_nanos",
				"compact_offset_date_time_calendar",
				"compact_offset_date_time_gregorian",
				"compact_offset_date_time_offset_nanos",
				"compact_offset_date_time_time_nanos",
				"unspecified_offset_date_time_calendar",
				"unspecified_offset_date_time_gregorian",
				"unspecified_offset_date_time_offset_nanos",
				"unspecified_offset_date_time_time_nanos",
				"complete_zoned_date_time_seconds",
				"complete_zoned_date_time_nanoseconds",
				"complete_zoned_date_time_zone_id",
				"complete_zoned_date_time_utc",
				"complete_zoned_date_time_local",
				"compact_zoned_date_time_zone_id",
				"compact_zoned_date_time_utc",
				"unspecified_zoned_date_time_zone_id",
				"unspecified_zoned_date_time_utc",
			};
			allCols.GroupBy(x => x).Where(x => x.Count() > 1).Should().BeEmpty();

			var connection = session.Connection;
			var command = connection.CreateCommand();
			command.CommandText = "SELECT * FROM storage_class_specified_compact_tests WHERE id = :id";
			var parameter = command.CreateParameter();
			parameter.ParameterName = "id";
			parameter.Value = savedId;
			command.Parameters.Add(parameter);
			using var reader = await command.ExecuteReaderAsync();
			var cols = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();
			cols.Should().HaveCount(allCols.Length);
			cols.Should().BeSubsetOf(allCols);
			allCols.Should().BeSubsetOf(cols);
		}
	}

	[Fact]
	public async Task VerifyColumnesForClassSpecifiedCompleteStorageMethod() {
		// Arrange
		var entity = new StorageClassSpecifiedCompleteTestEntity() { };

		// Act - Save
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		// Act - Verify in database (check columns were created)
		using (var session = _sessionFactory.OpenSession()) {
			var allCols = new string[]{
				"id",
				"complete_duration_seconds",
				"complete_duration_nanos",
				"compact_duration",
				"unspecified_duration_seconds",
				"unspecified_duration_nanos",

				"complete_instant_seconds",
				"complete_instant_nanoseconds",
				"complete_instant_timestamp",
				"compact_instant",
				"unspecified_instant_seconds",
				"unspecified_instant_nanoseconds",
				"unspecified_instant_timestamp",

				"complete_local_date_calendar",
				"complete_local_date_era",
				"complete_local_date_year",
				"complete_local_date_month",
				"complete_local_date_day",
				"complete_local_date_gregorian",
				"compact_local_date_calendar",
				"compact_local_date_gregorian",
				"unspecified_local_date_calendar",
				"unspecified_local_date_era",
				"unspecified_local_date_year",
				"unspecified_local_date_month",
				"unspecified_local_date_day",
				"unspecified_local_date_gregorian",

				"complete_local_date_time_calendar",
				"complete_local_date_time_era",
				"complete_local_date_time_year",
				"complete_local_date_time_month",
				"complete_local_date_time_day",
				"complete_local_date_time_gregorian",
				"complete_local_date_time_time_nanos",
				"compact_local_date_time_calendar",
				"compact_local_date_time_gregorian",
				"compact_local_date_time_time_nanos",
				"unspecified_local_date_time_calendar",
				"unspecified_local_date_time_era",
				"unspecified_local_date_time_year",
				"unspecified_local_date_time_month",
				"unspecified_local_date_time_day",
				"unspecified_local_date_time_gregorian",
				"unspecified_local_date_time_time_nanos",
				
				"complete_offset_date_calendar",
				"complete_offset_date_era",
				"complete_offset_date_year",
				"complete_offset_date_month",
				"complete_offset_date_day",
				"complete_offset_date_gregorian",
				"complete_offset_date_offset_nanos",
				"compact_offset_date_calendar",
				"compact_offset_date_gregorian",
				"compact_offset_date_offset_nanos",
				"unspecified_offset_date_calendar",
				"unspecified_offset_date_era",
				"unspecified_offset_date_year",
				"unspecified_offset_date_month",
				"unspecified_offset_date_day",
				"unspecified_offset_date_gregorian",
				"unspecified_offset_date_offset_nanos",
				
				"complete_offset_date_time_calendar",
				"complete_offset_date_time_era",
				"complete_offset_date_time_year",
				"complete_offset_date_time_month",
				"complete_offset_date_time_day",
				"complete_offset_date_time_gregorian",
				"complete_offset_date_time_time_nanos",
				"complete_offset_date_time_offset_nanos",
				"compact_offset_date_time_calendar",
				"compact_offset_date_time_gregorian",
				"compact_offset_date_time_offset_nanos",
				"compact_offset_date_time_time_nanos",
				"unspecified_offset_date_time_calendar",
				"unspecified_offset_date_time_era",
				"unspecified_offset_date_time_year",
				"unspecified_offset_date_time_month",
				"unspecified_offset_date_time_day",
				"unspecified_offset_date_time_gregorian",
				"unspecified_offset_date_time_time_nanos",
				"unspecified_offset_date_time_offset_nanos",
				
				"complete_zoned_date_time_seconds",
				"complete_zoned_date_time_nanoseconds",
				"complete_zoned_date_time_zone_id",
				"complete_zoned_date_time_utc",
				"complete_zoned_date_time_local",
				"compact_zoned_date_time_zone_id",
				"compact_zoned_date_time_utc",
				"unspecified_zoned_date_time_seconds",
				"unspecified_zoned_date_time_nanoseconds",
				"unspecified_zoned_date_time_zone_id",
				"unspecified_zoned_date_time_utc",
				"unspecified_zoned_date_time_local",

			};
			allCols.GroupBy(x => x).Where(x => x.Count() > 1).Should().BeEmpty();

			var connection = session.Connection;
			var command = connection.CreateCommand();
			command.CommandText = "SELECT * FROM storage_class_specified_complete_tests WHERE id = :id";
			var parameter = command.CreateParameter();
			parameter.ParameterName = "id";
			parameter.Value = savedId;
			command.Parameters.Add(parameter);
			using var reader = await command.ExecuteReaderAsync();
			var cols = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();
			cols.Should().HaveCount(allCols.Length);
			cols.Should().BeSubsetOf(allCols);
			allCols.Should().BeSubsetOf(cols);
		}
	}

	[Fact]
	public async Task VerifyColumnesForAssemblySpecifiedStorageMethod() {
		// Arrange
		var entity = new StorageUnspecifiedTestEntity() { };

		// Act - Save
		int savedId;
		using (var session = _sessionFactory.OpenSession())
		using (var transaction = session.BeginTransaction()) {
			savedId = (int)await session.SaveAsync(entity);
			await transaction.CommitAsync();
		}

		// Act - Verify in database (check columns were created)
		using (var session = _sessionFactory.OpenSession()) {
			var allCols = new string[]{
				"id",
				"complete_duration_seconds",
				"complete_duration_nanos",
				"compact_duration",
				"unspecified_duration_seconds",
				"unspecified_duration_nanos",

				"complete_instant_seconds",
				"complete_instant_nanoseconds",
				"complete_instant_timestamp",
				"compact_instant",
				"unspecified_instant_seconds",
				"unspecified_instant_nanoseconds",
				"unspecified_instant_timestamp",

				"complete_local_date_calendar",
				"complete_local_date_era",
				"complete_local_date_year",
				"complete_local_date_month",
				"complete_local_date_day",
				"complete_local_date_gregorian",
				"compact_local_date_calendar",
				"compact_local_date_gregorian",
				"unspecified_local_date_calendar",
				"unspecified_local_date_era",
				"unspecified_local_date_year",
				"unspecified_local_date_month",
				"unspecified_local_date_day",
				"unspecified_local_date_gregorian",

				"complete_local_date_time_calendar",
				"complete_local_date_time_era",
				"complete_local_date_time_year",
				"complete_local_date_time_month",
				"complete_local_date_time_day",
				"complete_local_date_time_gregorian",
				"complete_local_date_time_time_nanos",
				"compact_local_date_time_calendar",
				"compact_local_date_time_gregorian",
				"compact_local_date_time_time_nanos",
				"unspecified_local_date_time_calendar",
				"unspecified_local_date_time_era",
				"unspecified_local_date_time_year",
				"unspecified_local_date_time_month",
				"unspecified_local_date_time_day",
				"unspecified_local_date_time_gregorian",
				"unspecified_local_date_time_time_nanos",

				"complete_offset_date_calendar",
				"complete_offset_date_era",
				"complete_offset_date_year",
				"complete_offset_date_month",
				"complete_offset_date_day",
				"complete_offset_date_gregorian",
				"complete_offset_date_offset_nanos",
				"compact_offset_date_calendar",
				"compact_offset_date_gregorian",
				"compact_offset_date_offset_nanos",
				"unspecified_offset_date_calendar",
				"unspecified_offset_date_era",
				"unspecified_offset_date_year",
				"unspecified_offset_date_month",
				"unspecified_offset_date_day",
				"unspecified_offset_date_gregorian",
				"unspecified_offset_date_offset_nanos",

				"complete_offset_date_time_calendar",
				"complete_offset_date_time_era",
				"complete_offset_date_time_year",
				"complete_offset_date_time_month",
				"complete_offset_date_time_day",
				"complete_offset_date_time_gregorian",
				"complete_offset_date_time_time_nanos",
				"complete_offset_date_time_offset_nanos",
				"compact_offset_date_time_calendar",
				"compact_offset_date_time_gregorian",
				"compact_offset_date_time_offset_nanos",
				"compact_offset_date_time_time_nanos",
				"unspecified_offset_date_time_calendar",
				"unspecified_offset_date_time_era",
				"unspecified_offset_date_time_year",
				"unspecified_offset_date_time_month",
				"unspecified_offset_date_time_day",
				"unspecified_offset_date_time_gregorian",
				"unspecified_offset_date_time_time_nanos",
				"unspecified_offset_date_time_offset_nanos",

				"complete_zoned_date_time_seconds",
				"complete_zoned_date_time_nanoseconds",
				"complete_zoned_date_time_zone_id",
				"complete_zoned_date_time_utc",
				"complete_zoned_date_time_local",
				"compact_zoned_date_time_zone_id",
				"compact_zoned_date_time_utc",
				"unspecified_zoned_date_time_seconds",
				"unspecified_zoned_date_time_nanoseconds",
				"unspecified_zoned_date_time_zone_id",
				"unspecified_zoned_date_time_utc",
				"unspecified_zoned_date_time_local",

			};
			allCols.GroupBy(x => x).Where(x => x.Count() > 1).Should().BeEmpty();

			var connection = session.Connection;
			var command = connection.CreateCommand();
			command.CommandText = "SELECT * FROM storage_unspecified_tests WHERE id = :id";
			var parameter = command.CreateParameter();
			parameter.ParameterName = "id";
			parameter.Value = savedId;
			command.Parameters.Add(parameter);
			using var reader = await command.ExecuteReaderAsync();
			var cols = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();
			cols.Should().HaveCount(allCols.Length);
			cols.Should().BeSubsetOf(allCols);
			allCols.Should().BeSubsetOf(cols);
		}
	}
}
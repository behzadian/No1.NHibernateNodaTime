using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.Conventions;
using Testcontainers.PostgreSql;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Test fixture for InstantCompositeUserType using AutoMapping
/// </summary>
public class NHibernateCompositeTestFixture : IAsyncLifetime
{
	private PostgreSqlContainer? _container;
	private ISessionFactory? _sessionFactory;

	public ISessionFactory SessionFactory => _sessionFactory
		?? throw new InvalidOperationException("SessionFactory not initialized. Ensure InitializeAsync was called.");

	public async Task InitializeAsync() {
		// Create and start PostgreSQL container
		try {
			_container = new PostgreSqlBuilder()
				.WithImage("postgres:16-alpine")
				.WithDatabase("testdb")
				.WithUsername("testuser")
				.WithPassword("testpass")
				.WithCleanUp(true)
				.Build();

			await _container.StartAsync();
			Console.WriteLine($"Starting PG on port: {_container.GetMappedPublicPort(5432)}");
		} catch (Exception ex) {
			throw new InvalidOperationException(
				"Failed to start PostgreSQL test container. " +
				"Ensure Docker is running and accessible. " +
				"On Windows, start Docker Desktop. " +
				"On Linux, ensure Docker daemon is running and you have permissions. " +
				"On WSL2, enable WSL2 integration in Docker Desktop settings.",
				ex);
		}

		Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/hbms");
		// Configure NHibernate with AutoMapping and InstantCompositeUserType
		var nhibernateConfig = Fluently.Configure()
			.Database(PostgreSQLConfiguration.Standard
				.ConnectionString(_container.GetConnectionString())
				.ShowSql()
				.FormatSql())
			.Mappings(m => m
				.AutoMappings
				.Add(AutoMap
					.AssemblyOf<NHibernateCompositeTestFixture>(new TestAutoMappingConfiguration())
					.Conventions.Add<TableNameConvention>()
					.Conventions.Add<SnakeCaseColumnNameConvention>()
					.Conventions.Add<SnakeCaseForeignKeyConvention>()
					.Conventions.Add<SnakeCaseIdConvention>()
					.EnableNodaTime()
					.UseOverridesFromAssemblyOf<NHibernateCompositeTestFixture>()
				 )
				.ExportTo("hbms")
			)
			.ExposeConfiguration(cfg => {
				cfg.Properties[NHibernate.Cfg.Environment.PropertyUseReflectionOptimizer] = "false";

				// Create schema
				new SchemaExport(cfg).Create(true, true);
			})
			.BuildConfiguration();

		//var schemaExport = new SchemaExport(nhibernateConfig);
		//schemaExport.SetDelimiter(";");
		//schemaExport.Execute(useStdOut: true, execute: false, justDrop: false);

		_sessionFactory = nhibernateConfig.BuildSessionFactory();
	}

	public async Task DisposeAsync() {
		_sessionFactory?.Dispose();

		if (_container != null) {
			await _container.DisposeAsync();
		}
	}
}
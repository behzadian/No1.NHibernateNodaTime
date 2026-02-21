using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using No1.NHibernateNodaTime;
using Testcontainers.PostgreSql;
using Xunit;

namespace No1.NHibernateNodaTimeTests;

/// <summary>
/// Base fixture for NHibernate tests with PostgreSQL Testcontainer
/// </summary>
public class NHibernateTestFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    private ISessionFactory? _sessionFactory;

    public ISessionFactory SessionFactory => _sessionFactory
        ?? throw new InvalidOperationException("SessionFactory not initialized. Ensure InitializeAsync was called.");

    public async Task InitializeAsync()
    {
        // Create and start PostgreSQL container
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:15.1")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .WithCleanUp(true)
            .Build();

        await _container.StartAsync();

        // Configure NHibernate
        var configuration = Fluently.Configure()
            .Database(PostgreSQLConfiguration.Standard
                .ConnectionString(_container.GetConnectionString())
                .ShowSql()
                .FormatSql())
            .Mappings(m => m
                .FluentMappings.AddFromAssemblyOf<EventMap>()
                .Conventions.Add<InstantConvention>()) // Your custom convention
            .ExposeConfiguration(cfg =>
            {
                // Register custom user types if not using conventions
                cfg.Properties[NHibernate.Cfg.Environment.PropertyUseReflectionOptimizer] = "false";

                // Create schema
                new SchemaExport(cfg).Create(false, true);
            })
            .BuildConfiguration();

        _sessionFactory = configuration.BuildSessionFactory();
    }

    public async Task DisposeAsync()
    {
        _sessionFactory?.Dispose();

        if (_container != null)
        {
            await _container.DisposeAsync();
        }
    }
}

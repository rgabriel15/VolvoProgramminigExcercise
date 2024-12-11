using Base.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Base.Tests;
public sealed class ApiApplicationFactory : WebApplicationFactory<Program>
{
    #region Methods
    protected override IHost CreateHost(IHostBuilder builder)
    {
        _ = builder.ConfigureServices(services =>
        {
            _ = services
                .RemoveAll<DbContextOptions<EfContext>>()
                .RemoveAll<EfContext>();

            var loggerFactory = LoggerFactory
                .Create(builder => builder
                    .AddConsole()
                    .AddDebug());

            var databaseName = $"database-test-{Guid.NewGuid()}";
            var databaseRoot = new InMemoryDatabaseRoot();

            _ = services.AddDbContext<EfContext>(opt =>
                opt
                    .EnableSensitiveDataLogging()
                    .UseLoggerFactory(loggerFactory)
                    .UseInMemoryDatabase(
                        databaseName: databaseName
                        , databaseRoot: databaseRoot
                        , inMemoryOptionsAction: opt => opt.EnableNullChecks(false))
                    .EnableServiceProviderCaching(false)
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
        });

        return base.CreateHost(builder);
    }
    #endregion
}

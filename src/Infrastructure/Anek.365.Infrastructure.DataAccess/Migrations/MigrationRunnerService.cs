using Itmo.Dev.Platform.Postgres.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anek._365.Infrastructure.DataAccess.Migrations;

/// <summary>
///     Background service for applying migrations.
///     It will automatically apply migrations on ASP application start.
/// </summary>
public class MigrationRunnerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MigrationRunnerService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await scope.UsePlatformMigrationsAsync(stoppingToken);
    }
}
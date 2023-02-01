using Microsoft.Extensions.Hosting;

namespace MassTransit_ContextProviderBug;

public class MigrationsHostedService : IHostedService
{
    private readonly SomeDbContext _dbContext;

    public MigrationsHostedService(SomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
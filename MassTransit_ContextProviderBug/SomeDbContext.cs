using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MassTransit_ContextProviderBug;

public class SomeDbContext : DbContext
{
    public SomeDbContext(DbContextOptions<SomeDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        MapSomeSaga(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }

    static void MapSomeSaga(ModelBuilder modelBuilder)
    {
        var registration = modelBuilder.Entity<SomeSaga>();
        registration.HasKey(x => x.CorrelationId);
    }
}
using MassTransit;
using MassTransit_ContextProviderBug;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<SomeDbContext>(x =>
            {
                var connectionString = hostContext.Configuration.GetConnectionString("Default");

                x.UseNpgsql(connectionString);
            });

        services.AddHostedService<MigrationsHostedService>();

        services.AddMassTransit(x =>
        {
            if (Flags.UseOutbox)
            {
                x.AddEntityFrameworkOutbox<SomeDbContext>(o =>
                {
                    o.UsePostgres();

                    o.UseBusOutbox();
                });
            }

            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);

                cfg.UseSendFilter(typeof(SendFilter<>), context);
                cfg.UsePublishFilter(typeof(PublishFilter<>), context);
            });

            x.AddConsumer<SomeConsumer, SomeConsumerDefinition>()
                .Endpoint(e =>
                {
                    e.Name = "some-consumer";
                });

            x.AddSaga<SomeSaga, SomeSagaDefinition>()
                .Endpoint(e =>
                {
                    e.Name = "some-saga";
                })
                .EntityFrameworkRepository(r =>
                {
                    r.ExistingDbContext<SomeDbContext>();
                    r.UsePostgres();
                });
        });
    })
    .Build();

host.Start();

using (var scope = host.Services.CreateScope())
{
    var endpointProvider = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
    var endpoint = await endpointProvider.GetSendEndpoint(new Uri("queue:some-consumer"));
    await endpoint.Send(new CallConsumer());
    await scope.ServiceProvider.GetRequiredService<SomeDbContext>().SaveChangesAsync();
}

host.WaitForShutdown();
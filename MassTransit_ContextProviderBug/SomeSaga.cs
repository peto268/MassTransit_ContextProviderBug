using MassTransit;

namespace MassTransit_ContextProviderBug;

public class SomeSaga :
    ISaga,
    InitiatedBy<CallSaga>
{
    public Guid CorrelationId { get; set; }

    public Task Consume(ConsumeContext<CallSaga> context)
    {
        Console.WriteLine($"Handling {nameof(CallSaga)}");

        Environment.Exit(0);

        return Task.CompletedTask;
    }
}

public class SomeSagaDefinition : SagaDefinition<SomeSaga>
{
    private readonly IServiceProvider _serviceProvider;

    public SomeSagaDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<SomeSaga> sagaConfigurator)
    {
        base.ConfigureSaga(endpointConfigurator, sagaConfigurator);

        if (Flags.UseOutbox)
        {
            endpointConfigurator.UseEntityFrameworkOutbox<SomeDbContext>(_serviceProvider);
        }
    }
}

public class CallSaga : CorrelatedBy<Guid>
{
    public Guid CorrelationId => new();
}
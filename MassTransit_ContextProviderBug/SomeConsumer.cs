using MassTransit;

namespace MassTransit_ContextProviderBug;

public class SomeConsumer :
    IConsumer<CallConsumer>,
    IConsumer<CallConsumerFromConsumer>
{
    public async Task Consume(ConsumeContext<CallConsumer> context)
    {
        Console.WriteLine($"Handling {nameof(CallConsumer)}");

        if (Flags.UseSend)
        {
            await context.Send(new Uri("queue:some-consumer"), new CallConsumerFromConsumer());
        }
        else
        {
            await context.Publish(new CallConsumerFromConsumer());
        }
    }

    public async Task Consume(ConsumeContext<CallConsumerFromConsumer> context)
    {
        Console.WriteLine($"Handling {nameof(CallConsumerFromConsumer)}");

        if (Flags.UseSend)
        {
            await context.Send(new Uri("queue:some-saga"), new CallSaga());
        }
        else
        {
            await context.Publish(new CallSaga());
        }
    }
}

public class SomeConsumerDefinition : ConsumerDefinition<SomeConsumer>
{
    private readonly IServiceProvider _serviceProvider;

    public SomeConsumerDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<SomeConsumer> consumerConfigurator)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);

        if (Flags.UseOutbox)
        {
            endpointConfigurator.UseEntityFrameworkOutbox<SomeDbContext>(_serviceProvider);
        }
    }
}

public class CallConsumer
{
}

public class CallConsumerFromConsumer
{
}